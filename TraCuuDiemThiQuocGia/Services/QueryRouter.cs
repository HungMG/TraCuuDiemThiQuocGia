// Services/QueryRouter.cs
using Java.Net;
using Microsoft.Data.SqlClient;

public class QueryRouter
{
    // ======= CONFIG KẾT NỐI =======
    private readonly string _connBac =
        "Server=localhost,1433;Database=DiemThi_MienBac;User Id=sa;Password=YourPass;TrustServerCertificate=True;";

    private readonly string _connNam =
        "Server=localhost,1434;Database=DiemThi_MienNam;User Id=sa;Password=YourPass;TrustServerCertificate=True;";
    // ↑ Để giả lập sập: đổi 1434 → 9999 (port không tồn tại)

    private const int TIMEOUT_SECONDS = 3; // Timeout ngắn để phát hiện sập nhanh

    public async Task<KetQuaTraCuu> TraCuuAsync(int soBaoDanh)
    {
        // 1. QUERY ROUTER — Tính toán node nào chứa SBD này
        string connString;
        string tenNode;

        if (soBaoDanh >= 1 && soBaoDanh <= 500)
        {
            connString = _connBac;
            tenNode = "Miền Bắc";
        }
        else if (soBaoDanh >= 501 && soBaoDanh <= 1000)
        {
            connString = _connNam;
            tenNode = "Miền Nam";
        }
        else
        {
            return new KetQuaTraCuu
            {
                ThanhCong = false,
                ThongBao = "Số báo danh không hợp lệ (001–1000)"
            };
        }

        // 2. THỬ KẾT NỐI VÀ QUERY
        try
        {
            using var conn = new SqlConnection(connString);

            // Đặt timeout kết nối ngắn
            conn.ConnectionString += $"Connect Timeout={TIMEOUT_SECONDS};";

            await conn.OpenAsync(); // Nếu node sập → throw exception

            var cmd = new SqlCommand(
                "SELECT * FROM ThiSinh WHERE SoBaoDanh = @sbd", conn);
            cmd.Parameters.AddWithValue("@sbd", soBaoDanh);

            using var reader = await cmd.ExecuteReaderAsync();

            if (reader.Read())
            {
                return new KetQuaTraCuu
                {
                    ThanhCong = true,
                    ThiSinh = new ThiSinh
                    {
                        SoBaoDanh = reader.GetInt32(0),
                        HoTen = reader.GetString(1),
                        NgaySinh = reader.GetDateTime(2),
                        DiemToan = reader.GetDouble(3),
                        DiemVan = reader.GetDouble(4),
                        DiemAnh = reader.GetDouble(5),
                        KhuVuc = reader.GetString(6)
                    }
                };
            }
            else
            {
                return new KetQuaTraCuu
                {
                    ThanhCong = false,
                    ThongBao = $"Không tìm thấy SBD {soBaoDanh}"
                };
            }
        }
        // 3. XỬ LÝ LỖI — Node sập, không crash app
        catch (SqlException ex)
        {
            return new KetQuaTraCuu
            {
                ThanhCong = false,
                ThongBao = $"⚠️ Khu vực {tenNode} đang bảo trì. Vui lòng thử lại sau.\n(Lỗi: {ex.Number})"
            };
        }
        catch (TaskCanceledException)
        {
            return new KetQuaTraCuu
            {
                ThanhCong = false,
                ThongBao = $"⏱️ Khu vực {tenNode} không phản hồi (timeout). Vui lòng thử lại sau."
            };
        }
        catch (Exception ex)
        {
            return new KetQuaTraCuu
            {
                ThanhCong = false,
                ThongBao = $"❌ Lỗi hệ thống: {ex.Message}"
            };
        }
    }
}