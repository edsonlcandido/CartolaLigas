using CartolaLigas.Extensions;

namespace CartolaLigas.DTOs.Request
{
    internal class CreateLeagueRequest
    {
        public string name { get; set; }

        public string slug
        {
            get
            {
                return name.ToSlug();
            }
        }
    }
}
