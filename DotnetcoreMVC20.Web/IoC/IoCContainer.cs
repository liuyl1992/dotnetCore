using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetcoreMVC20.Web.Models.IoC
{
    /// <summary>
    /// Autofac IOC 容器
    /// </summary>
    public class IoCContainer
    {
        private static ContainerBuilder _builder = new ContainerBuilder();
        private static IContainer _container;
        private static string[] _otherAssembly;
        private static Dictionary<string, string> _dicAssembly = null;
        private static List<Type> _types = new List<Type>();
        private static Dictionary<Type, Type> _dicTypes = new Dictionary<Type, Type>();


        /// <summary>
        /// 注册程序集
        /// Node:同一程序集中同时存在接口和实现
        /// </summary>
        /// <param name="dicAssembly">字典：key:程序集全名value:程序集中类后缀</param>
        public static void Register(Dictionary<string, string> dicAssembly)
        {
            _dicAssembly = dicAssembly;
        }

        /// <summary>
        /// 注册程序集
        /// </summary>
        /// <param name="assemblies">程序集名称的集合</param>
        public static void Register(params string[] assemblies)
        {
            _otherAssembly = assemblies;
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="types"></param>
        public static void Register(params Type[] types)
        {
            _types.AddRange(types.ToList());
        }
        /// <summary>
        /// 注册程序集。
        /// </summary>
        /// <param name="implementationAssemblyName"></param>
        /// <param name="interfaceAssemblyName"></param>
        public static void Register(string implementationAssemblyName, string interfaceAssemblyName)
        {
            var implementationAssembly = Assembly.Load(implementationAssemblyName);
            var interfaceAssembly = Assembly.Load(interfaceAssemblyName);
            var implementationTypes =
                implementationAssembly.DefinedTypes.Where(t =>
                    t.IsClass && !t.IsAbstract && !t.IsGenericType && !t.IsNested);
            foreach (var type in implementationTypes)
            {
                var interfaceTypeName = interfaceAssemblyName + ".I" + type.Name;
                var interfaceType = interfaceAssembly.GetType(interfaceTypeName);
                if (interfaceType.IsAssignableFrom(type))
                {
                    _dicTypes.Add(interfaceType, type);
                }
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _dicTypes.Add(typeof(TInterface), typeof(TImplementation));
        }

        /// <summary>
        /// 注册一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void Register<T>(T instance) where T : class
        {
            _builder.RegisterInstance(instance).SingleInstance();
        }

        /// <summary>
        /// 构建IOC容器，需在各种Register后调用。
        /// </summary>
        public static IServiceProvider Build(IServiceCollection services)
        {
            if (_dicAssembly != null)
            {
                foreach (var item in _dicAssembly)
                {
                    _builder.RegisterAssemblyTypes(Assembly.Load(item.Key)).Where(t => t.Name.EndsWith(item.Value)).AsImplementedInterfaces();
                }
            }

            if (_otherAssembly != null)
            {
                foreach (var item in _otherAssembly)
                {
                    _builder.RegisterAssemblyTypes(Assembly.Load(item));
                }
            }

            if (_types.Any())
            {
                foreach (var type in _types)
                {
                    _builder.RegisterType(type);
                }
            }

            if (_dicTypes.Any())
            {
                foreach (var dicType in _dicTypes)
                {
                    _builder.RegisterType(dicType.Value).As(dicType.Key);
                }
            }

            _builder.Populate(services);
            _container = _builder.Build();
            return new AutofacServiceProvider(_container);
        }

        /// <summary>
        /// Resolve an instance of the default requested type from the container
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

    }
}