// Models/ThiSinh.cs
namespace TraCuuDiemThiQuocGia.Models;

public class ThiSinh
{
    public int SoBaoDanh { get; set; }
    public string HoTen { get; set; }
    public DateTime NgaySinh { get; set; }
    public double DiemToan { get; set; }
    public double DiemVan { get; set; }
    public double DiemAnh { get; set; }
    public string KhuVuc { get; set; }
    public double DiemTrungBinh => Math.Round((DiemToan + DiemVan + DiemAnh) / 3, 2);
}

public class KetQuaTraCuu
{
    public bool ThanhCong { get; set; }
    public string ThongBao { get; set; }
    public ThiSinh ThiSinh { get; set; }
}