
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;
using System;

namespace MonoGameToolkit
{
    public enum ContactType
    {
        Begin,
        End,
        PreSolve,
        PostSolve
    }
    
    public enum ContactDirection
    {
        None,
        Top,
        Bottom,
        Left,
        Right 
    }

    public struct ContactInfo
    {
        internal BaseObject _obj;
        public BaseObject Obj { get { return _obj; } }

        internal Manifold _manifold;
        public Manifold Manifold { get { return _manifold; } }

        internal ContactType _type;
        public ContactType Type { get { return _type; } }
        
        internal Vector2 _normal;
        public Vector2 Normal { get { return _normal; } }

        internal ContactDirection _direction;
        public ContactDirection Direction { get { return _direction; } }
        
        internal bool _invokeSensorCallback;
    }
}
