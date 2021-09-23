using Microsoft.AspNetCore.Http;
using API.Helpers;
using System.Text.Json;

namespace API.Exetentions
{
    public static class HttpExtentions
    {
        public static void AddPaginationHeaders(this HttpResponse response, int currentPage,
            int itemsPerPage, int totalItems, int totalPages)
        {
            var PaginationHeaders = new PaginationHeaders(currentPage, itemsPerPage, totalItems, totalPages);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            response.Headers.Add("Pagination", JsonSerializer.Serialize(PaginationHeaders,options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}