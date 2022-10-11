using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKGUIToolkit.Types
{
    public class TreeNode<T> : IEnumerable<TreeNode<T>>
    {

        public T Data { get; set; }
        public TreeNode<T> Parent { get; set; }
        public ICollection<TreeNode<T>> Children { get; set; }

        public event Nullable<Action<T>> CollectionChanged;

        /// <summary>
        /// Whether the node has no parent
        /// </summary>
        public bool IsRoot => Parent == null;


        /// <summary>
        /// Whether the node has no children
        /// </summary>
        public bool IsLeaf => Children.Count == 0;

        public int Level
        {
            get
            {
                if (this.IsRoot)
                    return 0;
                return Parent.Level + 1;
            }
        }


        public TreeNode(T data)
        {
            this.Data = data;
            this.Children = new LinkedList<TreeNode<T>>();

            this.ElementsIndex = new LinkedList<TreeNode<T>>();
            this.ElementsIndex.Add(this);
        }

        public TreeNode<T> AddChild(T child)
        {
            TreeNode<T> childNode = new TreeNode<T>(child) { Parent = this };
            this.Children.Add(childNode);

            this.RegisterChildForSearch(childNode);

            CollectionChanged?.Invoke(child);

            return childNode;
        }

        public override string ToString()
        {
            return Data != null ? Data.ToString() : "null";
        }


        #region searching

        private ICollection<TreeNode<T>> ElementsIndex { get; set; }

        private void RegisterChildForSearch(TreeNode<T> node)
        {
            ElementsIndex.Add(node);
            if (Parent != null)
                Parent.RegisterChildForSearch(node);
        }

        public TreeNode<T> FindTreeNode(Func<TreeNode<T>, bool> predicate)
        {
            return this.ElementsIndex.FirstOrDefault(predicate);
        }

        public List<TreeNode<T>> GetParents()
        {
            var parents = new List<TreeNode<T>>();

            TreeNode<T> currentNode = this.Parent;

            while (true)
            {

                if (currentNode != null && currentNode.Data != null)
                {
                    parents.Add(currentNode);
                    currentNode = currentNode.Parent;
                }
                else
                {
                    // no more parents, reached top
                    break;
                }
            }

            return parents;
        }

        #endregion


        #region iterating

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            yield return this;
            foreach (var directChild in this.Children)
            {
                foreach (var anyChild in directChild)
                    yield return anyChild;
            }
        }

        #endregion
    }
}