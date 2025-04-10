﻿namespace CartolaLigas.DTOs
{
    public class AuthResponse
    {
        public string token { get; set; }
        public UserDTO record { get; set; }
    }

    public class UserDTO
    {
        public string collectionId { get; set; }
        public string collectionName { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public bool emailVisibility { get; set; }
        public bool verified { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string role { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
    }
}