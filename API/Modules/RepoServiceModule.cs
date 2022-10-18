using Autofac;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWorks;
using Repository;
using Repository.Repositories;
using Repository.UnitOfWorks;
using Service.Mapping;
using Service.Services;
using System.Reflection;
using Module = Autofac.Module; //yazmaz isek Module Autofac'ten mi yoksa Reflection üzerinden mi geliyor anlamaz, hata verir.
namespace API.Modules
{
    public class RepoServiceModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Service<>)).As(typeof(IService<>)).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As(typeof(IUnitOfWork)).InstancePerLifetimeScope();
            //assembly'leri al.
            var apiAssembly = Assembly.GetExecutingAssembly();
            var repoAssembly = Assembly.GetAssembly(typeof(AppDbContext));
            var serviceAssembly = Assembly.GetAssembly(typeof(MapProfile));

            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly).Where(x => x.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerLifetimeScope();//burada assembly'lerimize git ve classların sonu "Repository" ile bitenleri al demek. InstancePerLifetimeScope bu metot addScoped'a karşılık gelir. perDependency ise Transient'e karşılık gelmektedir.
            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly).Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<ProductServiceWithNoCaching>().As<IProductService>();//IProductService'i gördüğünde ProductServiceWithNoCaching' in nesne örneğini alır.

        }
    }
}
