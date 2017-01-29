using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameToolkit
{
    internal class ContactListener
    {
        private List<KeyValuePair<ContactInfo, ContactInfo>> contacts = new List<KeyValuePair<ContactInfo, ContactInfo>>();

        internal ContactListener(ContactManager manager)
        {
            manager.BeginContact += BeginContact;
            manager.EndContact += EndContact;
            manager.PostSolve += PostSolve;
            manager.PreSolve += PreSolve;
        }

        internal bool BeginContact(Contact contact)
        {
            CreateContactInfo(contact, ContactType.Begin, ContactType.Begin);
            return true;
        }

        internal void EndContact(Contact contact)
        {
            CreateContactInfo(contact, ContactType.End, ContactType.End);
        }

        internal void PostSolve(Contact contact, ContactVelocityConstraint cvc)
        {
            //CreateContactInfo(contact, ContactType.PostSolve, ContactType.PostSolve);
        }

        internal void PreSolve(Contact contact, ref Manifold oldManifold)
        {
            //CreateContactInfo(contact, ContactType.PreSolve, ContactType.PreSolve);
        }

        private void CreateContactInfo(Contact contact, ContactType typeA, ContactType typeB)
        {
            ContactInfo a = new ContactInfo();
            a._obj = (BaseObject)contact.FixtureA.Body.UserData;
            a._manifold = contact.Manifold;
            a._type = typeA;

            ContactInfo b = new ContactInfo();
            b._obj = (BaseObject)contact.FixtureB.Body.UserData;
            b._manifold = contact.Manifold;
            b._type = typeB;

            b._invokeSensorCallback = contact.FixtureB.IsSensor;
            a._invokeSensorCallback = contact.FixtureA.IsSensor;

            FixedArray2<Vector2> points;
            contact.GetWorldManifold(out a._normal, out points);
            b._normal = -a._normal;

            if (a.Normal.Y == 1)
            {
                a._direction |= ContactDirection.Top;
                b._direction |= ContactDirection.Bottom;
            }
            else if (a.Normal.Y == -1)
            {
                a._direction |= ContactDirection.Bottom;
                b._direction |= ContactDirection.Top;
            }
            if (a.Normal.X == 1)
            {
                a._direction |= ContactDirection.Left;
                b._direction |= ContactDirection.Right;
            }
            else if (a.Normal.X == -1)
            {
                a._direction |= ContactDirection.Right;
                b._direction |= ContactDirection.Left;
            }

            contacts.Add(new KeyValuePair<ContactInfo, ContactInfo>(a, b));
        }
        
        internal void NotifyObjects()
        {
            foreach (KeyValuePair<ContactInfo, ContactInfo> contact in contacts)
            {
                InvokeCallback(contact.Key.Obj, contact.Key.Type, contact.Value);
                InvokeCallback(contact.Value.Obj, contact.Value.Type, contact.Key);
            }
            contacts.Clear();
        }

        private void InvokeCallback(BaseObject obj, ContactType contactType, ContactInfo contact)
        {
            if (!obj.PhysicsEnabled || !contact.Obj.PhysicsEnabled)
                return;

            switch (contactType)
            {
                case ContactType.Begin:
                    if (contact._invokeSensorCallback)
                        obj.OnContactSensorInternal(contact);
                    else
                        obj.OnContactInternal(contact);
                    break;
                case ContactType.End:
                    if (contact._invokeSensorCallback)
                        obj.OnSeperateSensorInternal(contact);
                    else
                        obj.OnSeperatedInternal(contact);
                    break;
                default:
                    break;
            }
        }
    }
}
