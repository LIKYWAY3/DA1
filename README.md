# 🎣 FuuFishing - Website Thương Mại Điện Tử Đồ Câu Cá

Dự án website thương mại điện tử chuyên cung cấp thiết bị và phụ kiện câu cá, xây dựng trên nền tảng **ASP.NET Core (.NET 10)** kết hợp kiến trúc MVC và RESTful Web API.

---

## 📌 1. Công Nghệ Sử Dụng

- **Backend Framework:** ASP.NET Core 10.0 (MVC + Web API)
- **ORM & Database Provider:** Entity Framework Core 10, Microsoft SQL Server
- **Authentication & Security:** ASP.NET Core Identity, JWT (JSON Web Tokens)
- **Containerization & Cloud Hosting:**
  - **Hosting Web:** Render (Linux Docker Container)
  - **Database Cloud:** Microsoft Azure SQL Database (Serverless)
  - **Local Database:** Microsoft SQL Server Express (`localhost\SQLEXPRESS`)

---

## 🚀 2. Hướng Dẫn Chạy Dự Án Dưới Máy Cục Bộ (Local)

### Yêu cầu tiên quyết:
- Đã cài đặt [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Đã cài đặt **SQL Server Express** trên máy tính

### Các bước khởi chạy:

1. **Clone repository về máy:**
   ```powershell
   git clone https://github.com/LIKYWAY3/DA1.git
   cd DA1
   ```

2. **Kiểm tra cấu hình chuỗi kết nối (`ASPtestShop/appsettings.json`):**
   ```json
   "ConnectionStrings": {
     "Hshop": "Server=localhost\\SQLEXPRESS;Database=Hshop;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True;"
   }
   ```

3. **Chạy ứng dụng:**
   Mở terminal tại thư mục gốc của dự án và chạy:
   ```powershell
   dotnet run --project ASPtestShop
   ```

4. **Truy cập ứng dụng:**
   - Giao diện khách hàng: `http://localhost:5198` hoặc `https://localhost:7198`
   - Đăng nhập Quản trị viên (Admin): `http://localhost:5198/admin/login`

---

## 🔑 3. Tài Khoản Quản Trị (Admin) Mặc Định

Hệ thống được khởi tạo sẵn tài khoản Admin dùng cho cả môi trường Local và Cloud:

| Thông tin | Giá trị |
|---|---|
| **Đường dẫn đăng nhập** | `/admin/login` |
| **Email** | `admin@gmail.com` |
| **Mật khẩu** | `Admin@123` |
| **Quyền hạn** | Quản lý sản phẩm, danh mục, đơn hàng, người dùng |

---

## 🔄 4. Quy Trình Làm Việc & Phát Triển (Workflow)

### A. Quản lý Sản phẩm / Dữ liệu hàng ngày:
> **Không cần sửa code hay can thiệp vào database!**
1. Truy cập vào trang web trên Render: `https://<tên-web-của-bạn>.onrender.com/admin/login`
2. Đăng nhập tài khoản Admin.
3. Vào mục **Quản lý sản phẩm** (`/admin/products`) để thêm, sửa giá, tải ảnh, cập nhật tồn kho hoặc xóa sản phẩm.
4. Dữ liệu sẽ được lưu thẳng vào Azure SQL và hiển thị ngay lập tức ra trang chủ cho khách hàng.

### B. Nâng cấp tính năng / Sửa code giao diện:
1. **Chỉnh sửa & Test tại máy (Local):**
   - Viết code, chỉnh sửa HTML/CSS/C# trên máy tính.
   - Chạy `dotnet run --project ASPtestShop` để kiểm tra trên trình duyệt cục bộ.
2. **Đẩy code lên GitHub:**
   ```powershell
   git add .
   git commit -m "Tính năng mới vừa làm"
   git push origin main
   ```
3. **Tự động cập nhật lên Render (CI/CD):**
   - Render tự động phát hiện commit mới trên nhánh `main`.
   - Render đọc file `Dockerfile`, build và deploy bản mới nhất (mất ~1-2 phút).
   - Dữ liệu sản phẩm trên Azure SQL vẫn giữ nguyên vẹn 100%.

### C. Khi có cập nhật cấu trúc Database (EF Core Migrations):
1. Mở terminal và tạo migration mới:
   ```powershell
   dotnet ef migrations add TenThayDoiMoi --project ASPtestShop
   ```
2. Commit và push lên GitHub.
3. Ứng dụng đã được cấu hình tự động chạy `await dbContext.Database.MigrateAsync();` khi khởi động trên Render, nên cơ sở dữ liệu trên Azure sẽ tự động cập nhật các bảng/cột mới.

---

## ☁️ 5. Cấu Hình Biến Môi Trường Trên Render (Production)

Trong mục **Environment** của Web Service trên [Render Dashboard](https://dashboard.render.com), cấu hình biến môi trường kết nối database Azure:

| Key | Value mẫu |
|---|---|
| `ConnectionStrings__Hshop` | `Server=tcp:fuufishing-server.database.windows.net,1433;Database=HshopDB;User ID=sqladmin;Password=<MAT_KHAU>;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;` |

---

## 📁 6. Cấu Trúc Thư Mục Dự Án

```text
├── ASPtestShop/
│   ├── Controllers/          # Xử lý Controller (MVC Admin & API khách hàng)
│   ├── Models/               # Entity Data Models & ViewModels
│   ├── Services/             # Xử lý nghiệp vụ (Product, Category, Auth, Order...)
│   ├── Views/                # Giao diện Razor Pages (Shop, Cart, Admin...)
│   ├── wwwroot/              # File tĩnh (CSS, JS, thư viện hình ảnh, Uploads)
│   ├── Program.cs            # Cấu hình DI, Middleware, Pipeline & Port
│   └── appsettings.json      # Cấu hình mặc định của ứng dụng
├── Dockerfile                # File cấu hình đóng gói container chạy trên Render (.NET 10)
├── .dockerignore             # Loại bỏ các file rác khi đóng gói Docker
├── hshop_clean.sql           # File SQL sao lưu dữ liệu sạch (24 danh mục + 100 sản phẩm mẫu)
└── README.md                 # Tài liệu hướng dẫn dự án
```

---

## 🛡️ 7. Lưu Ý Quan Trọng
- **Không xóa file `Dockerfile` và `.dockerignore`**: Render phụ thuộc vào 2 file này để build môi trường .NET 10.
- **Không cần mở SSMS khi code**: SQL Server chạy như một dịch vụ nền, ứng dụng tự động kết nối khi bạn chạy `dotnet run`.
- **Azure & Render chạy 24/7**: Bạn có thể tắt máy tính bất cứ lúc nào, website vẫn hoạt động liên tục trên mạng.
