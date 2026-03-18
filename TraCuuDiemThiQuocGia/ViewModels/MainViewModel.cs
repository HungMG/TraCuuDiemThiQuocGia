// ViewModels/MainViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TraCuuDiemThiQuocGia.Models;
using TraCuuDiemThiQuocGia.Services;

namespace TraCuuDiemThiQuocGia.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly QueryRouter _router = new QueryRouter();
    private string _soBaoDanh;
    private string _ketQua;
    private bool _isLoading;
    private ThiSinh _thiSinh;

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

    public ThiSinh ThiSinh
    {
        get => _thiSinh;
        set 
        { 
            _thiSinh = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsResultVisible));
            OnPropertyChanged(nameof(IsSearchVisible));
        }
    }

    public bool IsResultVisible => ThiSinh != null;
    public bool IsSearchVisible => ThiSinh == null;

    public ICommand TraCuuCommand => new Command(async () =>
    {
        if (!int.TryParse(SoBaoDanh, out int sbd))
        {
            KetQua = "⚠️ Vui lòng nhập số báo danh hợp lệ!";
            ThiSinh = null;
            return;
        }

        IsLoading = true;
        KetQua = "Đang tra cứu...";

        var result = await _router.TraCuuAsync(sbd);

        if (result.ThanhCong)
        {
            ThiSinh = result.ThiSinh;
            KetQua = $"""
                ✅ TÌM THẤY THÍ SINH
                ━━━━━━━━━━━━━━━━━━━━
                📋 SBD:     {result.ThiSinh.SoBaoDanh:D3}
                👤 Họ tên:  {result.ThiSinh.HoTen}
                🎂 Ngày SN: {result.ThiSinh.NgaySinh:dd/MM/yyyy}
                🌏 Khu vực: {result.ThiSinh.KhuVuc}
                ━━━━━━━━━━━━━━━━━━━━
                📐 Toán:    {result.ThiSinh.DiemToan}
                📝 Văn:     {result.ThiSinh.DiemVan}
                🌍 Anh:     {result.ThiSinh.DiemAnh}
                ⭐ TB:       {result.ThiSinh.DiemTrungBinh}
                """;
        }
        else
        {
            KetQua = result.ThongBao;
            ThiSinh = null;
        }

        IsLoading = false;
    });

    public ICommand ResetCommand => new Command(() =>
    {
        SoBaoDanh = "";
        KetQua = "";
        ThiSinh = null;
    });

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}