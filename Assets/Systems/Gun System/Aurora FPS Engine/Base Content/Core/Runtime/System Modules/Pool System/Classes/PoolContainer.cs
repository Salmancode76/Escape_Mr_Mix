/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System.Collections;
using UnityEngine;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using System.Collections.Generic;

namespace AuroraFPSRuntime.SystemModules
{
    [System.Serializable]
    public class PoolContainer : PoolContainerBase, IEnumerable, IEnumerable<PoolObject>
    {
        /// <summary>
        /// Represents base class for serialized Stack(PoolObject).
        /// </summary>
        [System.Serializable]
        public class PoolStack : SerializableStack<PoolObject>
        {
            /// <summary>
            /// Initializes a new instance of the serializable Stack(PoolObject) class that is empty and has the default initial capacity.
            /// </summary>
            public PoolStack() : base() { }

            /// <summary>
            /// Initializes a new instance of the serializable Stack(PoolObject) class that contains elements copied from the specified collection
            /// and has sufficient capacity to accommodate the number of elements copied.
            /// </summary>
            /// <param name="collection">Specified collection will copied to stack.</param>
            public PoolStack(IEnumerable<PoolObject> collection) : base(collection) { }

            /// <summary>
            /// Initializes a new instance of the serializable Stack(PoolObject) class that is empty and has the specified initial capacity
            /// or the default initial capacity, whichever is greater.
            /// </summary>
            /// <param name="capacity">Initial stack capacity.</param>
            public PoolStack(int capacity) : base(capacity) { }
        }


        // Base PoolContainer properties.
        [SerializeField] protected PoolStack objectsStack;
        [SerializeField] protected PoolObject original;
        [SerializeField] protected Allocator allocator;
        [SerializeField] protected int capacity;

        /// <summary>
        /// Initializes a new instance of the PoolContainer class.
        /// </summary>
        /// <param name="original">Object type of (PoolObject) that will contains in this container.</param>
        public PoolContainer(PoolObject original)
        {
            objectsStack = new PoolStack();
            this.original = original;
            this.allocator = Allocator.Dynamic;
            this.capacity = 1;
        }

        /// <summary>
        /// Initializes a new instance of the PoolContainer class.
        /// </summary>
        /// <param name="original">Object type of (PoolObject) that will contains in this container.</param>
        /// <param name="allocator">Container objects allocator type.</param>
        /// <param name="capacity">Reserved container capacity for objects.</param>
        public PoolContainer(PoolObject original, Allocator allocator, int capacity)
        {
            objectsStack = new PoolStack();
            this.original = original;
            this.allocator = allocator;
            this.capacity = capacity;
        }

        /// <summary>
        /// Push pool value in pool container.
        /// </summary>
        /// <param name="value">Value type of UnityEngine.Object.</param>
        public virtual void Push(PoolObject value)
        {
            if (value != null)
            {
                switch (allocator)
                {
                    case Allocator.Free:
                        value.gameObject.SetActive(false);
                        objectsStack.Push(value);
                        if(objectsStack.Count == capacity)
                        {
                            capacity++;
                        }
                        break;

                    case Allocator.Dynamic:
                    case Allocator.Fixed:
                        if (objectsStack.Count < capacity)
                        {
                            value.gameObject.SetActive(false);
                            objectsStack.Push(value);
                        }
                        else
                        {
                            Object.Destroy(value.gameObject);
                        }
                        break;

                }
                
            }
        }

        /// <summary>
        /// Get available value form pool container.
        /// </summary>
        /// <returns>
        /// Available pool value type of UnityEngine.Object.
        /// If pool container is empty, return the value depending on the type of pool container allocator.
        /// </returns>
        public virtual PoolObject Pop()
        {
            switch (allocator)
            {
                case Allocator.Free:
                case Allocator.Dynamic:
                    {
                        if(objectsStack.Count > 0)
                        {
                            PoolObject value = objectsStack.Pop();
                            value.gameObject.SetActive(true);
                            return value;
                        }
                        PoolObject newValue = Object.Instantiate(original);
                        return newValue;
                    }
                case Allocator.Fixed:
                    {
                        if(objectsStack.Count > 0)
                        {
                            PoolObject value = objectsStack.Pop();
                            value.gameObject.SetActive(true);
                            return value;
                        }
                        return null;
                    }

            }
            return null;
        }

        /// <summary>
        /// Try to peek value from container without remove it or creating new if is container is empty.
        /// 
        /// Note: value won't be activated after peek.
        /// </summary>
        /// <param name="value">
        /// First available pool value type of UnityEngine.Object.
        /// If the pool container is empty, the output value will be null.
        /// </param>
        /// <returns>True if container contains available object for peek, else false.</returns>
        public virtual bool TryPeek(out PoolObject value)
        {
            if (objectsStack.Count > 0)
            {
                value = objectsStack.Peek();
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Reallocate pool container type.
        /// </summary>
        /// <param name="allocator">Container allocator type.</param>
        /// <param name="size">Container size.</param>
        public virtual void Reallocate(Allocator allocator, int size)
        {
            this.allocator = allocator;
            this.capacity = size;
            if (objectsStack.Count > size)
            {
                int count = objectsStack.Count - size;
                for (int i = 0; i < count; i++)
                {
                    PoolObject value = objectsStack.Pop();
                    if (value != null)
                    {
                        Object.Destroy(value.gameObject);
                    }
                }
            }
            else if (objectsStack.Count < size)
            {
                int count = size - objectsStack.Count;
                for (int i = 0; i < count; i++)
                {
                    PoolObject value = Object.Instantiate(original);
                    if (value != null)
                    {
                        Push(value);
                    }
                }
            }

        }

        /// <summary>
        /// Length of available object in pool.
        /// </summary>
        public sealed override int GetLength()
        {
            return objectsStack.Count;
        }

        #region [IEnumerable / IEnumerable<T> Implementation]
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)objectsStack).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator(GameObject) that can be used to iterate through the collection.</returns>
        public IEnumerator<PoolObject> GetEnumerator()
        {
            return objectsStack.GetEnumerator();
        }
        #endregion

        #region [Getter / Setter]
        public PoolStack GetObjectsStack()
        {
            return objectsStack;
        }

        public PoolObject GetOriginal()
        {
            return original;
        }

        public Allocator GetAllocator()
        {
            return allocator;
        }

        public int GetSize()
        {
            return capacity;
        }
        #endregion
    }
}