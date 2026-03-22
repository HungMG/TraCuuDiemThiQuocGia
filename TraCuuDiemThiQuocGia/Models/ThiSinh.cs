namespace TraCuuDiemThiQuocGia.Models;

public class ThiSinh
{
    public int SoBaoDanh { get; set; }
    public string HoTen { get; set; }
    public DateTime NgaySinh { get; set; }
    public string KhuVuc { get; set; }  // "Miền Bắc" / "Miền Nam"
    public double DiemToan { get; set; }
    public double DiemVan { get; set; }
    public double DiemAnh { get; set; }

    // Tính tự động, không lưu trong DB
    public double DiemTrungBinh => Math.Round((DiemToan + DiemVan + DiemAnh) / 3.0, 2);
}