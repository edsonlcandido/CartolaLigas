using CartolaLigas.Models;

namespace CartolaLigas.DTOs.Response
{
    //"page": 1,
    //"perPage": 30,
    //"totalPages": 1,
    //"totalItems": 2,
    //"items": [
    //  {
    //    "collectionId": "pbc_4026191094",
    //    "collectionName": "ligas",
    //    "id": "test",
    //    "user_id": "RELATION_RECORD_ID",
    //    "name": "test",
    //    "slug": "test",
    //    "created": "2022-01-01 10:00:00.123Z",
    //    "updated": "2022-01-01 10:00:00.123Z"
    //  },
    //  {
    //  "collectionId": "pbc_4026191094",
    //    "collectionName": "ligas",
    //    "id": "[object Object]2",
    //    "user_id": "RELATION_RECORD_ID",
    //    "name": "test",
    //    "slug": "test",
    //    "created": "2022-01-01 10:00:00.123Z",
    //    "updated": "2022-01-01 10:00:00.123Z"
    //  }
    //]
    public class ListarLigasResponse
    {
        public int page { get; set; }
        public int perPage { get; set; }
        public int totalPages { get; set; }
        public int totalItems { get; set; }
        public List<Liga> items { get; set; }
    }
}
