using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.VeldridImplementation
{
    public class UniformBufferInfo : IDisposable
    {
        public required DeviceBuffer DeviceBuffer { get; init; }
        public required ResourceLayout ResourceLayout { get; init; }
        public required ResourceSet ResourceSet { get; init; }

        public static UniformBufferInfo Create<T>(ResourceFactory factory, string name, ShaderStages shaderStages)
        {
            var uniformBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)Unsafe.SizeOf<T>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription(name, ResourceKind.UniformBuffer, shaderStages)));

            var resourceSet = factory.CreateResourceSet(new ResourceSetDescription(resourceLayout, uniformBuffer));

            return new UniformBufferInfo
            {
                DeviceBuffer = uniformBuffer,
                ResourceLayout = resourceLayout,
                ResourceSet = resourceSet
            };
        }

        public void Dispose()
        {
            DeviceBuffer.Dispose();
            ResourceLayout.Dispose();
            ResourceSet.Dispose();
        }
    }

    public sealed class LightingBufferInfo : IDisposable
    {
        public required DeviceBuffer PointLightsBuffer { get; init; }
        public required DeviceBuffer DirectionalLightBuffer { get; init; }
        public required ResourceLayout ResourceLayout { get; init; }
        public required ResourceSet ResourceSet { get; init; }

        public static LightingBufferInfo Create(ResourceFactory factory, string name, ShaderStages shaderStages)
        {
            var pointLightsBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)Unsafe.SizeOf<PointLightsInfo>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var directionalLightBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)Unsafe.SizeOf<DirectionalLightInfo>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));


            var resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("PointLights", ResourceKind.UniformBuffer, shaderStages),
                new ResourceLayoutElementDescription("DirectionalLight", ResourceKind.UniformBuffer, shaderStages)));

            var resourceSet = factory.CreateResourceSet(new ResourceSetDescription(resourceLayout, pointLightsBuffer, directionalLightBuffer));

            return new LightingBufferInfo
            {
                PointLightsBuffer = pointLightsBuffer,
                DirectionalLightBuffer = directionalLightBuffer,
                ResourceLayout = resourceLayout,
                ResourceSet = resourceSet
            };
        }

        public void Dispose()
        {
            PointLightsBuffer.Dispose();
            DirectionalLightBuffer.Dispose();
            ResourceLayout.Dispose();
            ResourceSet.Dispose();
        }
    }
}
