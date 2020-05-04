﻿using Newtonsoft.Json;
using Nightingale.Core.Common;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.EventHandlers;
using Nightingale.Core.Workspaces.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nightingale.Core.Workspaces.Models
{
    /// <summary>
    /// Enum for the type of item
    /// this should be displayed in the tree.
    /// </summary>
    public enum ItemType
    {
        None,
        Request,
        Collection
    }

    /// <summary>
    /// This workspace item represents an object that
    /// can be displayed in the tree of the workspace.
    /// </summary>
    public class Item : ObservableBase, IEquatable<Item>, IComparable<Item>, IDeepCloneable<Item>
    {
        public Item()
        {
            // This updates the Parent properties
            // of all children added to this Item.
            Children.CollectionChanged += (sender, e) => ItemEventHandlers.CollectionChanged(sender, e, this);
        }

        public Item(bool childenObservable)
        {
            if (childenObservable)
            {
                // This updates the Parent properties
                // of all children added to this Item.
                Children.CollectionChanged += (sender, e) => ItemEventHandlers.CollectionChanged(sender, e, this);
            }
        }

        [JsonIgnore]
        public Item Parent { get; set; }

        /// <summary>
        /// A dictionary of properties usually
        /// used by a GUI app.
        /// </summary>
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public Url Url { get; set; } = new Url();

        public Authentication Auth { get; set; } = new Authentication();

        public RequestBody Body { get; set; } = new RequestBody();

        public ObservableCollection<Item> Children { get; } = new ObservableCollection<Item>();

        public ObservableCollection<Parameter> Headers { get; } = new ObservableCollection<Parameter>();

        public ObservableCollection<Parameter> ChainingRules { get; } = new ObservableCollection<Parameter>();

        private ItemType _type;
        public ItemType Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _method;
        public string Method
        {
            get => _method;
            set
            {
                _method = value;
                RaisePropertyChanged();
            }
        }

        private WorkspaceResponse _response;
        public WorkspaceResponse Response
        {
            get => _response;
            set
            {
                _response = value;
                RaisePropertyChanged();
            }
        }

        public override int GetHashCode()
        {
            return Method.GetHashCode() * 0x00010000
                * Name.GetHashCode() * 0x00010000
                * Type.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(Item other)
        {
            if (other is null) return 1;

            return Name.CompareTo(other.Name);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Item);
        }

        public bool Equals(Item other)
        {
            if (other is null)
            {
                return false;
            }

            return ReferenceEquals(this, other);
        }

        /// <inheritdoc/>
        public virtual Item DeepClone()
        {
            var other = new Item
            {
                Parent = this.Parent,
                Url = this.Url?.DeepClone() as Url,
                Auth = this.Auth?.DeepClone() as Authentication,
                Body = this.Body?.DeepClone() as RequestBody,
                Response = this.Response?.DeepClone() as WorkspaceResponse,
                Type = this.Type,
                Name = this.Name,
                IsExpanded = this.IsExpanded,
                Method = this.Method
            };
            other.Headers.DeepClone(this.Headers);
            other.ChainingRules.DeepClone(this.ChainingRules);
            other.Children.DeepClone(this.Children);
            return other;
        }

        public static bool operator ==(Item left, Item right)
        {
            // Check for null on left side.
            if (left is null)
            {
                if (right is null)
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Item left, Item right)
        {
            return !(left == right);
        }
    }
}
