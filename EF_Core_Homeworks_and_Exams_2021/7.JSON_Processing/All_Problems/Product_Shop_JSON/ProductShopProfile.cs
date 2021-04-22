using AutoMapper;
using ProductShop.DataTransferObjects;
using ProductShop.DataTransferObjects.Users;
using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //Query 2. - AutoMapp from UserInputModelDTO  to  User
            this.CreateMap<UserInputModelDTO, User>();

            //Query 3. - AutoMapp from ProductInputModelDTO  to  Product
            this.CreateMap<ProductInputModelDTO, Product>();

            //Query 4. - AutoMapp from CategoriesInputModelDTO  to  Category
            this.CreateMap<CategoriesInputModelDTO, Category>();

            //Query 5. - AutoMapp from CategoriesProductsInputModelDTO  to  CategoryProduct
            this.CreateMap<CategoriesProductsInputModelDTO, CategoryProduct >();

            // 7. Export Successfully Sold Products
            // first from Product to UserSoldProducsDTO (iiner DTO)
            this.CreateMap<Product, UserSoldProductsDTO>()
                .ForMember(x => x.BuyerFirstName, y => y.MapFrom(x => x.Buyer.FirstName))
                .ForMember(x => x.BuyerLastName, y => y.MapFrom(x => x.Buyer.LastName));
            // second from User to UserWithSoldProductsDTO (outher DTO)
            this.CreateMap<User, UserWithSoldProductsDTO>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x.ProductsSold.Where(p => p.Buyer != null)));
            // go to StartUp and replace the entire .Select() 
        }
    }
}
