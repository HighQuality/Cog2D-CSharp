using Cog;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Interface
{
    public interface IInterfaceElement
    {
        IInterfaceElement GenericParent { get; }
        Vector2 ScreenCoord { get; }
        Vector2 Location { get; }
        Vector2 Size { get; }
        Rectangle Bounds { get; }
        Rectangle ContentBounds { get; }
        Padding Padding { get; }

        bool PredicatePress(Mouse.Button button, Vector2 position);
        bool TriggerPress(Vector2 position, ButtonDownEvent ev);
        void OnPressed(Mouse.Button button, Vector2 position);
        void OnReleased(Mouse.Button button, Vector2 position);

        void TriggerUpdate(UpdateEvent ev);
        void TriggerDraw(DrawInterfaceEvent ev, Vector2 screenCoord);
        void OnUpdate(float deltaTime);
        void OnDraw(IRenderTarget target, Vector2 screenCoord);

        void ParentContentBoundsChanged(Rectangle newBounds);
        void ChildBoundsChanged(Rectangle newBounds);

        void AddChild(IInterfaceElement element);
    }

    public class InterfaceElement<TParent> : IInterfaceElement
        where TParent : class, IInterfaceElement
    {
        public TParent Parent { get; private set; }
        public IInterfaceElement GenericParent { get { return (IInterfaceElement)Parent; } }
        public Vector2 ScreenCoord { get { if (Parent != null) return Parent.ScreenCoord + Location; return Location; } }
        private Vector2 _location;
        public Vector2 Location
        {
            get { return _location; }
            set
            {
                if (value != _location)
                {
                    _location = value;
                    if (Parent != null)
                    {
                        if (_location.X < Parent.Padding.Left)
                            _location.X = Parent.Padding.Left;
                        if (_location.Y < Parent.Padding.Top)
                            _location.Y = Parent.Padding.Top;
                        if (_location.X + Size.X > Parent.Size.X - Parent.Padding.Right)
                            _location.X = Parent.Size.X - Parent.Padding.Right - Size.X;
                        if (_location.Y + Size.Y > Parent.Size.Y - Parent.Padding.Bottom)
                            _location.Y = Parent.Size.Y - Size.Y - Parent.Padding.Bottom;
                    }
                    var bounds = ContentBounds;
                    foreach (var element in children)
                        element.ParentContentBoundsChanged(bounds);
                    if (Parent != null)
                        Parent.ChildBoundsChanged(Bounds);
                }
            }
        }
        private Vector2 _size;
        public Vector2 Size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                {
                    _size = value;
                    if (_size.X < MinimumSize.X)
                        _size.X = MinimumSize.X;
                    if (_size.Y < MinimumSize.Y)
                        _size.Y = MinimumSize.Y;
                    if (Parent != null)
                    {
                        if (Location.X + _size.X > Parent.Size.X - Parent.Padding.Right)
                            _size.X = Parent.Size.X - Parent.Padding.Right - Location.X;
                        if (Location.Y + _size.Y > Parent.Size.Y - Parent.Padding.Bottom)
                            _size.Y = Parent.Size.Y - Parent.Padding.Bottom - Location.Y;
                    }
                    var bounds = ContentBounds;
                    foreach (var element in children)
                        element.ParentContentBoundsChanged(bounds);
                    if (Parent != null)
                        Parent.ChildBoundsChanged(Bounds);
                }
            }
        }
        public Rectangle Bounds
        {
            get { return new Rectangle(Location, Size); }
            set
            {
                if (value.TopLeft != Location || value.Size != Size)
                {
                    _location = value.TopLeft;
                    _size = value.Size;
                    var bounds = ContentBounds;
                    foreach (var child in EnumerateChildren())
                        child.ParentContentBoundsChanged(bounds);
                    if (Parent != null)
                        Parent.ChildBoundsChanged(Bounds);
                }
            }
        }
        public Rectangle ContentBounds { get { return new Rectangle(new Vector2(Padding.Left, Padding.Top), Size - new Vector2(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom)); } }
        public Padding Padding { get; set; }
        private List<IInterfaceElement> children = new List<IInterfaceElement>();
        private Vector2 _minimumSize;
        public Vector2 MinimumSize
        {
            get { return _minimumSize; }
            set
            {
                _minimumSize = value;

                if (_minimumSize.X > Size.X || _minimumSize.Y > Size.Y)
                    Size = _minimumSize;
            }
        }

        public InterfaceElement(TParent parent, Vector2 location)
        {
            this.Location = location;

            if (parent != null)
            {
                parent.AddChild(this);
                Parent = parent;
            }
        }

        public InterfaceElement(Vector2 location)
            : this(null, location)
        {
        }

        public void AddChild(IInterfaceElement element)
        {
            if (element.GenericParent != null)
                throw new InvalidOperationException("This InterfaceElement already has a parent!");
            children.Add(element);
        }

        public IEnumerable<T> EnumerateChildren<T>()
            where T : IInterfaceElement
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (children[i] is T)
                    yield return (T)children[i];
            }
        }

        public IEnumerable<IInterfaceElement> EnumerateChildren()
        {
            for (int i = children.Count - 1; i >= 0; i--)
                yield return children[i];
        }

        public virtual bool PredicatePress(Mouse.Button button, Vector2 position)
        {
            return IsInside(position);
        }

        public bool TriggerPress(Vector2 position, ButtonDownEvent ev)
        {
            if (position.X < 0f || position.Y < 0f || position.X >= Size.X || position.Y >= Size.Y)
                return false;

            foreach (var child in EnumerateChildren())
                if (child.PredicatePress(ev.Button, position - child.Location))
                    if (child.TriggerPress(position - child.Location, ev))
                        return true;

            OnPressed(ev.Button, position);
            ev.ButtonUpCallback = () => { OnReleased(ev.Button, Mouse.Location - ScreenCoord); };
            ev.Intercept = true;
            return true;
        }

        public virtual void OnPressed(Mouse.Button button, Vector2 position)
        {
        }

        public virtual void OnReleased(Mouse.Button button, Vector2 position)
        {
        }

        public void TriggerUpdate(UpdateEvent ev)
        {
            OnUpdate(ev.DeltaTime);

            foreach (var child in EnumerateChildren())
                child.TriggerUpdate(ev);
        }

        public void TriggerDraw(DrawInterfaceEvent ev, Vector2 screenCoord)
        {
            OnDraw(ev.RenderTarget, screenCoord);

            foreach (var child in EnumerateChildren())
                child.TriggerDraw(ev, screenCoord + child.Location);
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }

        public virtual void OnDraw(IRenderTarget target, Vector2 drawPosition)
        {
        }

        public virtual void ChildBoundsChanged(Rectangle bounds) { }
        public virtual void ParentContentBoundsChanged(Rectangle contentBounds) { }

        public virtual bool IsInside(Vector2 position)
        {
            return position.X >= 0f && position.Y >= 0f && position.X < Size.X && position.Y < Size.Y;
        }
    }

    public class InterfaceElement : InterfaceElement<IInterfaceElement>
    {
        public InterfaceElement(IInterfaceElement parent, Vector2 location)
            : base(parent, location)
        {
        }

        public InterfaceElement(Vector2 location)
            : base(null, location)
        {
        }
    }
}
