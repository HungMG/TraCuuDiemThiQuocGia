// ViewModels/MainViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly QueryRouter _router = new QueryRouter();
    private string _soBaoDanh;
    private string _ketQua;
    private bool _isLoading;

    public string SoBaoDanh
    {
        get => _soBaoDanh;
        set { _soBaoDanh = value; OnPropertyChanged(); }
    }

    public string KetQua
    {
        get => _ketQua;
        set { _ketQua = value; OnPropertyChanged(); }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public ICommand TraCuuCommand => new Command(async () =>
    {
        if (!int.TryParse(SoBaoDanh, out int sbd))
        {
            KetQua = "⚠️ Vui lòng nhập số báo danh hợp lệ!";
            return;
        }

        IsLoading = true;
        KetQua = "Đang tra cứu...";

        var result = await _router.TraCuuAsync(sbd);

        if (result.ThanhCong)
        {
            var ts = result.ThiSinh;
            KetQua = $"""
                ✅ TÌM THẤY THÍ SINH
                ━━━━━━━━━━━━━━━━━━━━
                📋 SBD:     {ts.SoBaoDanh:D3}
                👤 Họ tên:  {ts.HoTen}
                🎂 Ngày SN: {ts.NgaySinh:dd/MM/yyyy}
                🌏 Khu vực: {ts.KhuVuc}
                ━━━━━━━━━━━━━━━━━━━━
                📐 Toán:    {ts.DiemToan}
                📝 Văn:     {ts.DiemVan}
                🌍 Anh:     {ts.DiemAnh}
                ⭐ TB:       {ts.DiemTrungBinh}
                """;
        }
        else
        {
            KetQua = result.ThongBao;
        }

        IsLoading = false;
    });

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}