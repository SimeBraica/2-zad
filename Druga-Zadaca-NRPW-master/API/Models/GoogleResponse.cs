namespace API.Models {
    public class GoogleResponse {
            public bool Success { get; set; }
            public double Score { get; set; }
            public string Action { get; set; }
            public string Challenge_ts { get; set; }
            public string Hostname { get; set; }
            public string ErrorCodes { get; set; }
    }

}
