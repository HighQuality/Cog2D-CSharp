using Cog;
using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
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
        bool TriggerPress(Mouse.Button button, Vector2 position, ButtonDownEvent ev);
        void OnPressed(Mouse.Button button, Vector2 position);
        void OnReleased(Mouse.Button button, Vector2 position);

        void ParentContentBoundsChanged(Rectangle newBounds);
        void ChildBoundsChanged(Rectangle newBounds);
    }

    public class InterfaceElement<TParent> : IInterfaceElement
        where TParent : class, IInterfaceElement
    {
        public TParent Parent { get; private set; }
        public IInterfaceElement GenericParent { get { return (IInterfaceElement)Parent; } }
        public Vector2 ScreenCoord { get { if (Parent != null) return Parent.ScreenCoord + Location; return Location; } }
        private Vector2 _location;
        public Vector2 Location { get { return _location; } set { _location = value; var bounds = ContentBounds; foreach (var element in children) element.ParentContentBoundsChanged(bounds); if (Parent != null) Parent.ChildBoundsChanged(Bounds); } }
        private Vector2 _size;
        public Vector2 Size { get { return _size; } set { _size = value; var bounds = ContentBounds; foreach (var element in children) element.ParentContentBoundsChanged(bounds); if (Parent != null) Parent.ChildBoundsChanged(Bounds); } }
        public Rectangle Bounds { get { return new Rectangle(Location, Size); } private set { _location = value.TopLeft; _size = value.Size; var bounds = ContentBounds; foreach (var child in EnumerateChildren()) child.ParentContentBoundsChanged(bounds); if (Parent != null) Parent.ChildBoundsChanged(Bounds); } }
        public Rectangle ContentBounds { get { return new Rectangle(Location + new Vector2(Padding.Left, Padding.Top), Location + Size - new Vector2(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom)); } }
        public Padding Padding { get; private set; }
        private List<IInterfaceElement> children = new List<IInterfaceElement>();

        public InterfaceElement(TParent parent, Vector2 location)
        {
            this.Parent = parent;
            this.Location = location;
        }

        public InterfaceElement(Vector2 location)
            : this(null, location)
        {
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

        public bool TriggerPress(Mouse.Button button, Vector2 position, ButtonDownEvent ev)
        {
            if (!Bounds.Contains(position))
                return false;

            foreach (var child in EnumerateChildren())
            {
                if (child.PredicatePress(button, position))
                {

                }
            }

            OnPressed(button, position);
            ev.ButtonUpCallback = () => { OnReleased(button, Mouse.Location - ScreenCoord); };
            return true;
        }

        public virtual void OnPressed(Mouse.Button button, Vector2 position)
        {
        }

        public virtual void OnReleased(Mouse.Button button, Vector2 position)
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
