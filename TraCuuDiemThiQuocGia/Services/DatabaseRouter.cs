using System.Net.Http;
using System.Text.Json;
using TraCuuDiemThiQuocGia.Models;

namespace TraCuuDiemThiQuocGia.Services;

public class DatabaseRouter
{
    private static readonly HttpClient _http = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private const string API_URL = "https://stauroscopically-unlethargical-merideth.ngrok-free.dev/api/tracuu";

    public static async Task<(ThiSinh ketQua, string loi)> TraCuuAsync(int sbd)
    {
        try
        {
            var response = await _http.GetAsync($"{API_URL}/{sbd}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ThiSinhJson>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var ts = new ThiSinh
                {
                    SoBaoDanh = data.SoBaoDanh,
                    HoTen = data.HoTen,
                    NgaySinh = data.NgaySinh,
                    KhuVuc = data.KhuVuc,
                    DiemToan = data.DiemToan,
                    DiemVan = data.DiemVan,
                    DiemAnh = data.DiemAnh,
                };
                return (ts, null);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var msg = await response.Content.ReadAsStringAsync();
                return (null, msg);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                var msg = await response.Content.ReadAsStringAsync();
                return (null, msg); // "Khu vực đang bảo trì"
            }
            else
            {
                return (null, $"Lỗi server: {response.StatusCode}");
            }
        }
        catch (TaskCanceledException)
        {
            return (null, "Không kết nối được server. Vui lòng kiểm tra mạng.");
        }
        catch (Exception ex)
        {
            return (null, $"Lỗi: {ex.Message}");
        }
    }

    private class ThiSinhJson
    {
        public int SoBaoDanh { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string KhuVuc { get; set; }
        public double DiemToan { get; set; }
        public double DiemVan { get; set; }
        public double DiemAnh { get; set; }
    }
}