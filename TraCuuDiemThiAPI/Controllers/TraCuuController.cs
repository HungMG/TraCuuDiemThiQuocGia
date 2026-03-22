using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TraCuuDiemThiAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraCuuController : ControllerBase
{
    private const string CONN_MIEN_BAC =
        "Server=HungMG\\SQLEXPRESS;Database=DB_MienBac;" +
        "Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=5;";

    private const string CONN_MIEN_NAM =
        "Server=HungMG\\SQLEXPRESS;Database=DB_MienNam;" +
        "Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=5;";

    [HttpGet("{sbd}")]
    public async Task<IActionResult> TraCuu(int sbd)
    {
        string connStr, khuVuc;

        if (sbd >= 1 && sbd <= 500)
            (connStr, khuVuc) = (CONN_MIEN_BAC, "Miền Bắc");
        else if (sbd >= 501 && sbd <= 1000)
            (connStr, khuVuc) = (CONN_MIEN_NAM, "Miền Nam");
        else
            return BadRequest("SBD phải từ 1 đến 1000.");

        try
        {
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            const string sql = "SELECT * FROM ThiSinh WHERE MaSoThiSinh = @SBD";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SBD", sbd);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read())
                return NotFound($"Không tìm thấy thí sinh có SBD {sbd}.");

            return Ok(new
            {
                soBaoDanh = reader.GetInt32(0),
                hoTen = reader.GetString(1),
                ngaySinh = reader.GetDateTime(2),
                khuVuc = khuVuc,
                diemToan = reader.GetDouble(4),
                diemVan = reader.GetDouble(5),
                diemAnh = reader.GetDouble(6),
            });
        }
        catch (SqlException)
        {
            return StatusCode(503, $"Khu vực {khuVuc} đang bảo trì. Vui lòng thử lại sau.");
        }
    }
}