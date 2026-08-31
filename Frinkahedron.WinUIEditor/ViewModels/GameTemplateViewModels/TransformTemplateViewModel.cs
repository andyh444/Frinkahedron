using CommunityToolkit.Mvvm.ComponentModel;
using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels
{
    public sealed class TransformTemplateViewModel(TransformTemplate model) : ViewModelBase
    {
        public TransformTemplate Model => model;

        public float TranslationX
        {
            get => model.Translation.X;
            set => SetModelProperty(model.Translation.X, value, v => model.Translation = new Vector3(v, model.Translation.Y, model.Translation.Z));
        }

        public float TranslationY
        {
            get => model.Translation.Y;
            set => SetModelProperty(model.Translation.Y, value, v => model.Translation = new Vector3(model.Translation.X, v, model.Translation.Z));
        }

        public float TranslationZ
        {
            get => model.Translation.Z;
            set => SetModelProperty(model.Translation.Z, value, v => model.Translation = new Vector3(model.Translation.X, model.Translation.Y, v));
        }

        public float RotationX
        {
            get => model.RotationEulerAngles.X;
            set => SetModelProperty(model.RotationEulerAngles.X, value, v => model.RotationEulerAngles = new Vector3(v, model.RotationEulerAngles.Y, model.RotationEulerAngles.Z));
        }

        public float RotationY
        {
            get => model.RotationEulerAngles.Y;
            set => SetModelProperty(model.RotationEulerAngles.Y, value, v => model.RotationEulerAngles = new Vector3(model.RotationEulerAngles.X, v, model.RotationEulerAngles.Z));
        }

        public float RotationZ
        {
            get => model.RotationEulerAngles.Z;
            set => SetModelProperty(model.RotationEulerAngles.Z, value, v => model.RotationEulerAngles = new Vector3(model.RotationEulerAngles.X, model.RotationEulerAngles.Y, v));
        }

        public float ScaleX
        {
            get => model.Scale.X;
            set => SetModelProperty(model.Scale.X, value, v => model.Scale = new Vector3(v, model.Scale.Y, model.Scale.Z));
        }

        public float ScaleY
        {
            get => model.Scale.Y;
            set => SetModelProperty(model.Scale.Y, value, v => model.Scale = new Vector3(model.Scale.X, v, model.Scale.Z));
        }

        public float ScaleZ
        {
            get => model.Scale.Z;
            set => SetModelProperty(model.Scale.Z, value, v => model.Scale = new Vector3(model.Scale.X, model.Scale.Y, v));
        }
    }
}
