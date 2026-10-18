using Microsoft.EntityFrameworkCore;
using BinaryTechHub.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

// 2. Database context (Clean setup without retry strategy conflicts)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// 4. Auto-create schema and seed the entire 67-item catalog via direct SQL
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    try
    {
        db.Database.ExecuteSqlRaw(@"
            INSERT IGNORE INTO products (product_id, category_id, name, price, stock, specs, image_url) VALUES
            ('SP-001', 1, 'Apple iPhone 18 Pro', 27999.00, 12, '256GB, A20 Pro chip, ProMotion OLED display', 'images/iphone-18-pro.webp'),
            ('SP-002', 1, 'Apple iPhone 18 Pro Max', 31999.00, 10, '512GB, Periscope telephoto, Titanium frame', 'images/iphone-18-promax.webp'),
            ('SP-003', 1, 'Samsung Galaxy S26', 21499.00, 15, '256GB, Dynamic AMOLED 2X, Snapdragon Gen 5', 'images/galaxy-s26.jpg'),
            ('SP-004', 1, 'Samsung Galaxy S26+', 24999.00, 12, '512GB, 4900mAh battery, 45W fast charge', 'images/galaxy-s26-plus.jpg'),
            ('SP-005', 1, 'Samsung Galaxy S26 Ultra', 30499.00, 8, '512GB, Embedded S-Pen, 200MP Quad Camera', 'images/galaxy-s26-ultra.jpg'),
            ('SP-006', 1, 'Samsung Galaxy S26 FE', 15999.00, 20, '128GB, Flagship-tier camera, 120Hz display', 'images/galaxy-s26-fe.avif'),
            ('SP-007', 1, 'Google Pixel 11', 18499.00, 14, '128GB, Tensor G6 chip, Next-gen Google AI', 'images/pixel-11.webp'),
            ('SP-008', 1, 'Google Pixel 11 Pro', 23999.00, 9, '256GB, Super Actua display, Pro camera controls', 'images/pixel-11-pro.webp'),
            ('SP-009', 1, 'Xiaomi 17', 16999.00, 16, '256GB, Leica optical lens, 90W HyperCharge', 'images/xiaomi-17.webp'),
            ('SP-010', 1, 'Xiaomi 17 Ultra', 26499.00, 7, '512GB, 1-inch sensor, Variable aperture', 'images/xiaomi-17-ultra.jpeg'),
            ('SP-011', 1, 'Huawei Nova 14', 11499.00, 18, '256GB, 60MP portrait front cam, SuperCharge', 'images/huawei-nova-14.jpeg'),
            ('LC-001', 2, 'Apple MacBook Neo', 17999.00, 14, '13-inch, Apple Silicon, Fanless ultra-portable design', 'images/macbook-neo.jpg'),
            ('LC-002', 2, 'Apple MacBook Air M5', 22999.00, 15, '16GB RAM, 512GB SSD, Liquid Retina display', 'images/macbook_air_m5.png'),
            ('LC-003', 2, 'Apple MacBook Pro 16 M5 Pro', 38999.00, 8, '24GB Unified Memory, 1TB SSD, 120Hz XDR', 'images/macbook-pro-m5-pro.webp'),
            ('LC-004', 2, 'Apple MacBook Pro 16 M5 Max', 54999.00, 5, '36GB Unified Memory, 1TB SSD, Extreme GPU power', 'images/macbook-pro-m5-max.png'),
            ('LC-005', 2, 'ASUS Zenbook Duo (2026)', 34999.00, 6, 'Dual 14-inch OLED touch displays, Intel Core Ultra 9', 'images/zenbook-duo-2026.jpg'),
            ('LC-006', 2, 'MSI Claw 8 EX AI+', 16999.00, 11, '8-inch 120Hz FHD, 32GB LPDDR5x, 80Wh battery', 'images/msi-claw-8.jpg'),
            ('LC-007', 2, 'Apple Mac mini M6', 13999.00, 12, '16GB Unified Memory, 512GB SSD, Ultra-compact desktop', 'images/mac-mini-m6.webp'),
            ('TB-001', 3, 'Apple iPad Air 13 M4', 14499.00, 14, '13-inch Liquid Retina, Apple Pencil Pro support', 'images/ipad-air-m4.jpg'),
            ('TB-002', 3, 'Apple iPad Pro 13-inch', 27999.00, 7, '256GB, Tandem OLED display, M4 architecture', 'images/ipad-pro-13.jpeg'),
            ('TB-003', 3, 'Samsung Galaxy Tab S12 Ultra', 26499.00, 6, '14.6-inch Dynamic AMOLED 2X, S-Pen included', 'images/tab-s12-ultra.avif'),
            ('TB-004', 3, 'Samsung Galaxy Tab S12+', 20999.00, 9, '12.4-inch 120Hz screen, 512GB storage, S-Pen', 'images/tab-s12-plus.webp'),
            ('TB-005', 3, 'Xiaomi Pad 9 Pro', 11999.00, 15, '144Hz 3K display, Quad stereo speakers, Snapdragon', 'images/xiaomi-pad-9-pro.jpeg'),
            ('TB-006', 3, 'OnePlus Pad 3', 10499.00, 12, '7:5 ReadFit display ratio, 67W SUPERVOOC charging', 'images/oneplus-pad-3.jpg'),
            ('AU-001', 4, 'Apple AirPods 5', 3999.00, 30, 'Active Noise Cancellation, Adaptive Audio, USB-C case', 'images/airpods-5.jpg'),
            ('AU-002', 4, 'Apple AirPods Max 2', 12499.00, 10, 'Lossless USB-C audio, Personalized Spatial Audio', 'images/apple-airpods-max-2.png'),
            ('AU-003', 4, 'Sony WH-1000XM6', 8499.00, 16, 'Industry-leading ANC, 30-hour battery, LDAC Hi-Res', 'images/sony-wh1000xm6.webp'),
            ('AU-004', 4, 'JBL Live 780NC', 3499.00, 25, 'True Adaptive Noise Cancelling, 65-hour playtime', 'images/jbl-live-780nc.webp'),
            ('AU-005', 4, 'Bose QuietComfort Ultra', 8999.00, 12, 'Bose Immersive Audio, World-class active noise cancellation', 'images/bose-qc-ultra.jpg'),
            ('AU-006', 4, 'Samsung Galaxy Buds3 Pro', 3799.00, 28, 'Blade design, 24-bit Hi-Fi sound, Galaxy AI noise control', 'images/galaxy-buds3-pro.jpg'),
            ('WT-001', 5, 'Apple Watch Ultra 3', 19999.00, 8, '49mm Titanium case, Dual-frequency GPS, 72h battery', 'images/apple-watch-ultra-3.jpg'),
            ('WT-002', 5, 'Apple Watch Series 11', 10499.00, 18, 'Thinner design, Sleep apnea alerts, Fast charging', 'images/apple-watch-series-11.png'),
            ('WT-003', 5, 'Samsung Galaxy Watch 8 Classic', 8999.00, 15, 'Physical rotating bezel, BioActive Sensor, Sapphire crystal', 'images/galaxy-watch-8-classic.jpg'),
            ('WT-004', 5, 'Huawei Watch GT 6 Pro', 7499.00, 14, 'Titanium alloy build, 14-day battery life, ECG analysis', 'images/huawei-gt6-pro.jpg'),
            ('WT-005', 5, 'Huawei Watch Ultimate 2', 15999.00, 6, 'Zirconium liquid metal case, 100m diving certification', 'images/huawei-watch-ultimate-2.jpeg'),
            ('WT-006', 5, 'Google Pixel Watch 5', 7999.00, 12, 'Actua display, Advanced Fitbit biometric tracking engine', 'images/pixel-watch-5.webp'),
            ('SPK-001', 6, 'JBL Xtreme 5', 6499.00, 15, 'IP67 waterproof, AI sound boost, replaceable battery', 'images/jbl-xtreme-5.jpg'),
            ('SPK-002', 6, 'JBL Go 5', 899.00, 40, 'Ultra-compact, 7h playtime, integrated carabiner strap', 'images/jbl-go-5.jpg'),
            ('SPK-003', 6, 'JBL PartyBox 330', 11999.00, 8, '240W output, dynamic synchronized lightshow, mic inputs', 'images/jbl-partybox-330.webp'),
            ('SPK-004', 6, 'Samsung HW-Q990F Soundbar', 21999.00, 6, '11.1.4 Channel Dolby Atmos, Wireless Q-Symphony', 'images/samsung-hw-q990f.jpg'),
            ('SPK-005', 6, 'Sony BRAVIA Theatre System 6', 18499.00, 5, '360 Spatial Sound Mapping, Wireless rear sub package', 'images/sony-theatre-6.webp'),
            ('SPK-006', 6, 'Bose Lifestyle Ultra System', 32999.00, 4, 'OmniJewel satellite speakers, Acoustimass wireless bass', 'images/bose-lifestyle-ultra.png'),
            ('TV-001', 7, 'Samsung 65"" S95H OLED TV', 39999.00, 5, 'Glare-free QD-OLED panel, 144Hz 4K gaming, NQ4 AI Gen3', 'images/samsung-s95h-oled.jpeg'),
            ('TV-002', 7, 'LG 65"" C6 OLED TV', 36499.00, 6, 'Alpha 10 AI processor, 4x HDMI 2.1, G-Sync / FreeSync', 'images/lg-c6-oled.jpg'),
            ('TV-003', 7, 'Hisense 65"" U7N Mini-LED TV', 16999.00, 10, '144Hz Game Mode Pro, Full array local dimming, HDR10+', 'images/hisense-u7n.jpg'),
            ('TV-004', 7, 'Sony 65"" BRAVIA 8 OLED TV', 37999.00, 4, 'Acoustic Surface Audio+, XR Processor, PS5 auto HDR', 'images/sony-bravia-8.webp'),
            ('TV-005', 7, 'TCL 65"" C855 Premium QD-Mini LED', 18999.00, 8, '3500 nits peak brightness, ONKYO 2.2.2 Hi-Fi audio', 'images/tcl-c855.jpeg'),
            ('GM-001', 8, 'Sony PlayStation 5 Pro', 16499.00, 10, '2TB SSD, Advanced Ray Tracing, PSSR 4K 60FPS upscaling', 'images/ps5-pro.webp'),
            ('GM-002', 8, 'Microsoft Xbox Series X', 13499.00, 12, 'True 4K 120FPS, Quick Resume, Xbox Velocity Architecture', 'images/xbox-series-x.jpg'),
            ('GM-003', 8, 'Nintendo Switch 2', 8999.00, 15, '8-inch 1080p display, DLSS dock mode, Joy-Con 2 controllers', 'images/switch-2.webp'),
            ('GM-004', 8, 'Meta Quest 3 (512GB)', 14999.00, 9, 'High-res color passthrough, Snapdragon XR2 Gen 2 VR/MR', 'images/meta-quest-3.webp'),
            ('GM-005', 8, 'Logitech G Pro X Superlight 2', 2799.00, 25, 'LIGHTFORCE hybrid optical switches, 60g ultralight, HERO 2', 'images/gpro-x-superlight-2.jpg'),
            ('CD-001', 9, 'Sony A7R VI Mirrorless Camera', 69999.00, 3, '61MP Exmor R sensor, AI-assisted autofocus, 8K recording', 'images/sony-a7r-vi.jpg'),
            ('CD-002', 9, 'DJI Osmo Pocket 4', 9499.00, 12, '1-inch CMOS sensor, 3-axis gimbal, 4K 120FPS OLED', 'images/dji-pocket-4.jpg'),
            ('CD-003', 9, 'GoPro Mission 1 Pro', 8499.00, 15, '5.3K 60FPS, HyperSmooth stabilization, 10m waterproof', 'images/gopro-mission-1.jpeg'),
            ('CD-004', 9, 'DJI Mini 5 Pro Drone', 17999.00, 8, 'Sub-249g lightweight, Omnidirectional obstacle sensing', 'images/dji-mini-5-pro.jpeg'),
            ('CD-005', 9, 'DJI Avata 360 FPV Drone', 22499.00, 5, '360-degree immersive video capture, Goggles 3 bundle', 'images/dji-avata-360.webp'),
            ('CD-006', 9, 'Insta360 Link 2 4K Webcam', 4299.00, 14, 'AI tracking gimbal, 1/2-inch sensor, gesture controls', 'images/insta360-link-2.jpeg'),
            ('PRN-001', 10, 'Bambu Lab P2S Combo', 19999.00, 6, 'Multi-color AMS printing, enclosed chamber, 500mm/s speed', 'images/bambu-p2s.png'),
            ('PRN-002', 10, 'Bambu Lab H-series', 26499.00, 4, 'High-temperature engineering filament support, active heat', 'images/bambu-h-series.jpg'),
            ('PRN-003', 10, 'Bambu Lab X1 Carbon', 28999.00, 5, 'Micro Lidar leveling, Dual AI cameras, Carbon fiber body', 'images/bambu-x1-carbon.jpg'),
            ('PRN-004', 10, 'Bambu Lab A1 Combo', 11499.00, 10, 'AMS Lite multi-color printing, active motor noise canceling', 'images/bambu-a1-combo.png'),
            ('PRN-005', 10, 'Creality K2 Pro', 17499.00, 7, '600mm/s CoreXY, CFS multi-color system, AI camera', 'images/creality-k2-pro.webp'),
            ('PRN-006', 10, 'Creality K2 Plus', 21999.00, 5, 'Large 350x350x350mm build volume, heated chamber', 'images/creality-k2-plus.jpg'),
            ('PRN-007', 10, 'Creality Ender-5 Max', 12999.00, 8, 'Cube frame stability, 300x300x400mm print volume', 'images/creality-ender-5-max.jpg'),
            ('PRN-008', 10, 'Creality K1C High-Speed', 11999.00, 12, 'Carbon fiber filament nozzle, 600mm/s max print speed', 'images/creality-k1c.webp'),
            ('AC-001', 11, 'Anker Prime 20,000mAh Power Bank', 2299.00, 25, '200W total output, smart digital display, dual USB-C PD', 'images/anker-prime-pb.jpg'),
            ('AC-002', 11, 'UGREEN Nexode 20,000mAh Power Bank', 1699.00, 22, '130W fast charge, TFT color screen, multi-device charge', 'images/ugreen-nexode-pb.jpg'),
            ('AC-003', 11, 'Logitech MX Keys S Keyboard', 2199.00, 20, 'Smart illumination, Perfect Stroke keys, multi-device pair', 'images/logitech-mx-keys-s.jpg'),
            ('AC-004', 11, 'Razer BlackWidow V4 Pro Keyboard', 4499.00, 12, 'Mechanical Green switches, Command Dial, Chroma RGB', 'images/razer-blackwidow-v4.jpg'),
            ('AC-005', 11, 'ASUS RT-BE96U Wi-Fi 7 Router', 12999.00, 6, 'Quad-band Wi-Fi 7, 19,000 Mbps bandwidth, dual 10G ports', 'images/asus-rt-be96u.jpg'),
            ('AC-006', 11, 'TP-Link Deco BE85 Mesh (2-Pack)', 18499.00, 5, 'Whole-home Wi-Fi 7 mesh, 22 Gbps, 10G SFP+/Ethernet', 'images/tplink-deco-be85.jpg'),
            ('AC-007', 11, 'Epson EcoTank ET-4950 Printer', 7999.00, 9, 'Cartridge-free ink tank, ADF scanner, Ethernet & Wi-Fi', 'images/epson-et-4950.jpg'),
            ('AC-008', 11, 'Canon MegaTank G3270 Printer', 3999.00, 14, 'Refillable ink system, wireless printing, 3-in-1 print/copy', 'images/canon-g3270.jpg'),
            ('AC-009', 11, 'Creality Hyper PLA Filament (1kg)', 399.00, 50, 'High-speed 600mm/s printing, 1.75mm diameter, tough', 'images/creality-pla-filament.jpg'),
            ('AC-010', 11, 'Bambu Lab PETG Basic Filament (1kg)', 449.00, 45, 'Water-resistant, high impact resistance, smooth finish', 'images/bambu-petg-filament.jpg'),
            ('AC-011', 11, 'eSUN ABS+ 3D Filament (1kg)', 429.00, 35, 'High strength and heat resistance, modified ABS formula', 'images/esun-abs-filament.jpg'),
            ('AC-012', 11, 'Overture TPU Flexible Filament (1kg)', 549.00, 30, '95A shore hardness, elastic, high-wear resistance', 'images/overture-tpu-filament.jpg'),
            ('AC-013', 11, 'Anycubic Standard UV Resin (1kg)', 599.00, 25, '405nm LCD photopolymer resin, high precision, low odor', 'images/anycubic-uv-resin.jpg'),
            ('AC-014', 11, 'Bambu Lab Carbon-Fiber PLA (1kg)', 699.00, 20, 'Carbon fiber reinforced, high stiffness, matte finish', 'images/bambu-cf-filament.jpg'),
            ('AC-015', 11, 'Samsung T9 Portable SSD (2TB)', 3999.00, 18, 'USB 3.2 Gen 2x2, up to 2000MB/s read/write speed', 'images/samsung-t9-ssd.jpg'),
            ('AC-016', 11, 'SanDisk Extreme Portable SSD (2TB)', 3499.00, 20, 'IP65 water/dust resistance, drop protection, 1050MB/s', 'images/sandisk-extreme-ssd.jpg');
        ");
    }
    catch
    {
        // Fail silently if rows already exist or columns are configured differently
    }
}

app.Run();