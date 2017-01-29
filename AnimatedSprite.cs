using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameToolkit
{
    [Flags]
    public enum AnimationState
    {
        Stopped = 0,
        Paused = 1,
        Playing = 2,
        PlayingBackward = 4,
        PlayingPingPong = 8
    }

    public class AnimatedSprite : Sprite
    {
        public const AnimationState PlayingMask = AnimationState.Playing | AnimationState.PlayingBackward | AnimationState.PlayingPingPong;

        private Dictionary<string, KeyFrame[]> _animations = new Dictionary<string, KeyFrame[]>();
        public Dictionary<string, KeyFrame[]> Animations
        {
            get { return _animations; }
            set { _animations = value; }
        }

        private float _animationSpeed;
        /// <summary>
        /// The time it takes 1 frame to play (in seconds).
        /// </summary>
        public float AnimationSpeed
        {
            get { return _animationSpeed; }
            set { _animationSpeed = value; }
        }
        
        private int _animationFrame;
        public int AnimationFrame
        {
            get { return _animationFrame; }
            set { _animationFrame = value; }
        } 
        
        private AnimationState _animationState;
        public AnimationState AnimationState { get { return _animationState; } }

        private string _currentAnimation;
        public string CurrentAnimation { get { return _currentAnimation; } } 

        /// <summary>
        /// Returns true if an animation is playing, (forward, backward or ping pong).
        /// </summary>
        public bool IsPlayingAnimation { get { return ((AnimationState & PlayingMask) != 0); } }
        
        private AnimationState _animateStateBeforePause;
        private int _currentAnimationLength;
        private TimeSpan _timer;
        private bool _ping;

        public AnimatedSprite(string texture, Dictionary<string, KeyFrame[]> animations)
            : base(texture)
        {
            _animations = animations;
            _animationState = AnimationState.Stopped;
            _animateStateBeforePause = AnimationState.Stopped;
            _animationSpeed = 0.1f;
            _ping = true;
            _timer = TimeSpan.Zero;
        }

        public AnimatedSprite(Texture2D texture, Dictionary<string, KeyFrame[]> animations)
            : base(texture) 
        {
            _animations = animations;
            _animationState = AnimationState.Stopped;
            _animateStateBeforePause = AnimationState.Stopped;
            _animationSpeed = 0.1f;
            _ping = true;
            _timer = TimeSpan.Zero;
        }

        public AnimatedSprite(Texture2D texture)
            : this(texture, new Dictionary<string, KeyFrame[]>()) { }

        public AnimatedSprite()
            : this(MGTK.Instance.DefaultTexture, new Dictionary<string, KeyFrame[]>()) { }

        public AnimatedSprite(string texture)
            : this(texture, new Dictionary<string, KeyFrame[]>()) { }

        public void PlayAnimation(string animation)
        {
            if(!(_currentAnimation == animation && _animationState == AnimationState.Playing))
                Play(animation, AnimationState.Playing, 0);
        }

        public void PlayAnimation(string animation, int startIndex)
        {
            if (!(_currentAnimation == animation && _animationState == AnimationState.Playing))
                Play(animation, AnimationState.Playing, startIndex);
        }

        public void PlayAnimationBackwards(string animation)
        {
            if (!(_currentAnimation == animation && _animationState == AnimationState.PlayingBackward))
                Play(animation, AnimationState.PlayingBackward, Animations[animation].Length - 1);
        }

        public void PlayAnimationBackwards(string animation, int startIndex)
        {
            if (!(_currentAnimation == animation && _animationState == AnimationState.PlayingBackward))
                Play(animation, AnimationState.PlayingBackward, startIndex);
        }
        
        public void PlayAnimationPingPong(string animation)
        {
            if (!(_currentAnimation == animation && _animationState == AnimationState.PlayingPingPong))
                Play(animation, AnimationState.PlayingPingPong, 0);
        }

        public void PlayAnimationPingPong(string animation, int startIndex)
        {
            if (!(_currentAnimation == animation && _animationState == AnimationState.PlayingPingPong))
                Play(animation, AnimationState.PlayingPingPong, startIndex);
        }

        public void PauseAnimation()
        {
            if (_animationState != AnimationState.Paused)
            {
                _animateStateBeforePause = _animationState;
                _animationState = AnimationState.Paused;
            }
        }

        public void ResumeAnimation()
        {
            if(_animationState == AnimationState.Paused)
                _animationState = _animateStateBeforePause;
        }

        public void StopAnimation()
        {
            if(_animationState != AnimationState.Stopped)
            {
                if (!string.IsNullOrEmpty(_currentAnimation))
                {
                    KeyFrame frame = _animations[_currentAnimation][0];
                    SourceRect = frame.SourceRect;
                    Origin = frame.Origin;
                }
                SetCurrentAnimation(string.Empty);
                _animationFrame = 0;
                _ping = true;
                _timer = TimeSpan.Zero;
                _animationState = AnimationState.Stopped;
            }
        }

        private void Play(string animation, AnimationState state, int startIndex)
        {
            StopAnimation();
            SetCurrentAnimation(animation);
            _animationState = state;
            _animationFrame = startIndex;
            _timer = TimeSpan.Zero;
            if (startIndex >= Animations[_currentAnimation].Length)
                throw new Exception("[AnimatedSprite] startIndex out of range");
            KeyFrame frame = _animations[_currentAnimation][_animationFrame];
            SourceRect = frame.SourceRect;
            Origin = frame.Origin;
        }

        protected override void Update(GameTime gameTime)
        {
            int nextIndex = -1;
            switch (_animationState)
            {
                case AnimationState.Stopped:
                    break;
                case AnimationState.Paused:
                    break;
                case AnimationState.Playing:
                    nextIndex = (_animationFrame + 1) < _currentAnimationLength ? _animationFrame + 1 : 0;
                    Animate(nextIndex, gameTime.ElapsedGameTime);
                    break;
                case AnimationState.PlayingBackward:
                    nextIndex = (_animationFrame - 1) >= 0 ? _animationFrame - 1 : _currentAnimationLength - 1;
                    Animate(nextIndex, gameTime.ElapsedGameTime);
                    break;
                case AnimationState.PlayingPingPong:
                    nextIndex = _ping ? _animationFrame + 1 : _animationFrame - 1;
                    Animate(nextIndex, gameTime.ElapsedGameTime);
                    if (_animationFrame == _currentAnimationLength - 1 && _ping)
                        _ping = false;
                    else if (_animationFrame == 0 && !_ping)
                        _ping = true;
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        private void Animate(int nextIndex, TimeSpan elapsed)
        {
            KeyFrame[] frames = _animations[_currentAnimation];
            if (_timer.TotalSeconds >= AnimationSpeed)
            {
                _animationFrame = nextIndex;
                KeyFrame frame = frames[_animationFrame];
                SourceRect = frame.SourceRect;
                Origin = frame.Origin;
                _timer = TimeSpan.Zero;
            }
            _timer += elapsed;
        }

        private void SetCurrentAnimation(string current)
        {
            _currentAnimation = current;
            if(!string.IsNullOrEmpty(_currentAnimation))
                _currentAnimationLength = Animations[_currentAnimation].Length;
        }
    }
}
