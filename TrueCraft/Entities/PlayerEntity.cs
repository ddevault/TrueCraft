using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Entities
{
    public class PlayerEntity : LivingEntity
    {
        public PlayerEntity(string username) : base()
        {
            Username = username;
            Food = 20;
        }

        public const double Width = 0.6;
        public const double Height = 1.62;
        public const double Depth = 0.6;

        public override Size Size
        {
            get { return new Size(Width, Height, Depth); }
        }

        public override short MaxHealth
        {
            get { return 20; }
        }

        public string Username { get; set; }
        public bool IsSprinting { get; set; }
        public bool IsCrouching { get; set; }
        public double PositiveDeltaY { get; set; }

        protected short _SelectedSlot;
        public short SelectedSlot
        {
            get { return _SelectedSlot; }
            set
            {
                _SelectedSlot = value;
                OnPropertyChanged("SelectedSlot");
            }
        }

        public ItemStack ItemInMouse { get; set; }

        protected Vector3 _SpawnPoint;
        public Vector3 SpawnPoint
        {
            get { return _SpawnPoint; }
            set
            {
                _SpawnPoint = value;
                OnPropertyChanged("SpawnPoint");
            }
        }

        protected short _Food;
        public short Food
        {
            get { return _Food; }
            set
            {
                _Food = value;
                OnPropertyChanged("Food");
            }
        }

        protected float _FoodSaturation;
        public float FoodSaturation
        {
            get { return _FoodSaturation; }
            set
            {
                _FoodSaturation = value;
                OnPropertyChanged("FoodSaturation");
            }
        }

        protected float _FoodExhaustion;
        public float FoodExhaustion
        {
            get { return _FoodExhaustion; }
            set
            {
                _FoodExhaustion = value;
                OnPropertyChanged("FoodExhaustion");
            }
        }
    }
}
