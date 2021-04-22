using AutoMapper;
using ProductShop.DTO.Exp;
using ProductShop.DTO.Imp;
using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            // -- Import -- // from DTO to Entity
            // Query 1. Import Users
            this.CreateMap<ImportUsersDTO, User>();

            // Query 2. Import Products
            this.CreateMap<ImportProductsDTO, Product>();

            // Query 3. Import Categories
            this.CreateMap<ImportCategoriesDTO, Category>();

            // Query 4. Import Categories and Products
            this.CreateMap<ImportCategoryProductsDTO, CategoryProduct>();


            // -- EXPORT -- // from Entity to DTO
            // Query 5. Products In Range
            this.CreateMap<Product, ExportProductsInRangeDTO>()
                .ForMember(x => x.Buyer, y => y.MapFrom(s => s.Buyer.FirstName + " " + s.Buyer.LastName));

            // Query 6. Sold Products
            this.CreateMap<Product, ProductsSoldDTO>();

            this.CreateMap<User, ExportSoldProductsDTO>();

            // Query 7. Categories By Products Count
            // manual mapping - ok -
            this.CreateMap<Category, ExportCategoriesByProductsCountDTO>();

            // automapper -> with ProjectTo<ExportCategoriesByProductsCountDTO>(mapper.ConfigurationProvider);
            //this.CreateMap<Category, ExportCategoriesByProductsCountDTO>()
            //    .ForMember(x => x.Count, y => y.MapFrom(s => s.CategoryProducts.Count))
            //    .ForMember(x => x.AveragePrice, y => y.MapFrom(s => s.CategoryProducts.Average(p => p.Product.Price)))
            //    .ForMember(x => x.TotalRevenue, y => y.MapFrom(s => s.CategoryProducts.Sum(p => p.Product.Price)));


            // Query 8. Users and Products
            //this.CreateMap<User, RootUsersWithProductsDTO>();

        }
    }
}
