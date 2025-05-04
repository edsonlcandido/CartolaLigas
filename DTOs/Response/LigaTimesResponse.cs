namespace CartolaLigas.DTOs.Response
{

    public class LigaTimesResponse
    {
        public List<CartolaLigas.Models.LigaTime> Items { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}