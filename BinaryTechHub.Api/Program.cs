using Microsoft.EntityFrameworkCore;
using BinaryTechHub.Api.Data;
using BinaryTechHub.Api.Models;

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

// 4. Auto-create schema and seed Categories + Products into Aiven MySQL
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    try
    {
        // 1. Ensure Categories exist first (so Foreign Keys are satisfied)
        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS categories (
                category_id INT AUTO_INCREMENT PRIMARY KEY,
                name VARCHAR(100) NOT NULL
            );
            INSERT IGNORE INTO categories (category_id, name) VALUES
            (1, 'Smartphones'),
            (2, 'Laptops and Computers'),
            (3, 'Tablets'),
            (4, 'Headphones and Earphones'),
            (5, 'Smartwatches'),
            (6, 'Speakers and Sound Systems'),
            (7, 'Televisions'),
            (8, 'Gaming'),
            (9, 'Cameras and Drones'),
            (10, '3D Printers'),
            (11, 'Accessories and Other Electronics');
        ");

        // 2. Seed all 67 Products if empty
        if (!db.Products.Any())
        {
            db.Products.AddRange(
                // 1. Smartphones
                new Product { ProductId = "SP-001", CategoryId = 1, Name = "Apple iPhone 18 Pro", Price = 27999.00m, Stock = 12, Specs = "256GB, A20 Pro chip, ProMotion OLED display", ImageUrl = "images/iphone-18-pro.webp" },
                new Product { ProductId = "SP-002", CategoryId = 1, Name = "Apple iPhone 18 Pro Max", Price = 31999.00m, Stock = 10, Specs = "512GB, Periscope telephoto, Titanium frame", ImageUrl = "images/iphone-18-promax.webp" },
                new Product { ProductId = "SP-003", CategoryId = 1, Name = "Samsung Galaxy S26", Price = 21499.00m, Stock = 15, Specs = "256GB, Dynamic AMOLED 2X, Snapdragon Gen 5", ImageUrl = "images/galaxy-s26.jpg" },
                new Product { ProductId = "SP-004", CategoryId = 1, Name = "Samsung Galaxy S26+", Price = 24999.00m, Stock = 12, Specs = "512GB, 4900mAh battery, 45W fast charge", ImageUrl = "images/galaxy-s26-plus.jpg" },
                new Product { ProductId = "SP-005", CategoryId = 1, Name = "Samsung Galaxy S26 Ultra", Price = 30499.00m, Stock = 8, Specs = "512GB, Embedded S-Pen, 200MP Quad Camera", ImageUrl = "images/galaxy-s26-ultra.jpg" },
                new Product { ProductId = "SP-006", CategoryId = 1, Name = "Samsung Galaxy S26 FE", Price = 15999.00m, Stock = 20, Specs = "128GB, Flagship-tier camera, 120Hz display", ImageUrl = "images/galaxy-s26-fe.avif" },
                new Product { ProductId = "SP-007", CategoryId = 1, Name = "Google Pixel 11", Price = 18499.00m, Stock = 14, Specs = "128GB, Tensor G6 chip, Next-gen Google AI", ImageUrl = "images/pixel-11.webp" },
                new Product { ProductId = "SP-008", CategoryId = 1, Name = "Google Pixel 11 Pro", Price = 23999.00m, Stock = 9, Specs = "256GB, Super Actua display, Pro camera controls", ImageUrl = "images/pixel-11-pro.webp" },
                new Product { ProductId = "SP-009", CategoryId = 1, Name = "Xiaomi 17", Price = 16999.00m, Stock = 16, Specs = "256GB, Leica optical lens, 90W HyperCharge", ImageUrl = "images/xiaomi-17.webp" },
                new Product { ProductId = "SP-010", CategoryId = 1, Name = "Xiaomi 17 Ultra", Price = 26499.00m, Stock = 7, Specs = "512GB, 1-inch sensor, Variable aperture", ImageUrl = "images/xiaomi-17-ultra.jpeg" },
                new Product { ProductId = "SP-011", CategoryId = 1, Name = "Huawei Nova 14", Price = 11499.00m, Stock = 18, Specs = "256GB, 60MP portrait front cam, SuperCharge", ImageUrl = "images/huawei-nova-14.jpeg" },

                // 2. Laptops and Computers
                new Product { ProductId = "LC-001", CategoryId = 2, Name = "Apple MacBook Neo", Price = 17999.00m, Stock = 14, Specs = "13-inch, Apple Silicon, Fanless ultra-portable design", ImageUrl = "images/macbook-neo.jpg" },
                new Product { ProductId = "LC-002", CategoryId = 2, Name = "Apple MacBook Air M5", Price = 22999.00m, Stock = 15, Specs = "16GB RAM, 512GB SSD, Liquid Retina display", ImageUrl = "images/macbook_air_m5.png" },
                new Product { ProductId = "LC-003", CategoryId = 2, Name = "Apple MacBook Pro 16 M5 Pro", Price = 38999.00m, Stock = 8, Specs = "24GB Unified Memory, 1TB SSD, 120Hz XDR", ImageUrl = "images/macbook-pro-m5-pro.webp" },
                new Product { ProductId = "LC-004", CategoryId = 2, Name = "Apple MacBook Pro 16 M5 Max", Price = 54999.00m, Stock = 5, Specs = "36GB Unified Memory, 1TB SSD, Extreme GPU power", ImageUrl = "images/macbook-pro-m5-max.png" },
                new Product { ProductId = "LC-005", CategoryId = 2, Name = "ASUS Zenbook Duo (2026)", Price = 34999.00m, Stock = 6, Specs = "Dual 14-inch OLED touch displays, Intel Core Ultra 9", ImageUrl = "images/zenbook-duo-2026.jpg" },
                new Product { ProductId = "LC-006", CategoryId = 2, Name = "MSI Claw 8 EX AI+", Price = 16999.00m, Stock = 11, Specs = "8-inch 120Hz FHD, 32GB LPDDR5x, 80Wh battery", ImageUrl = "images/msi-claw-8.jpg" },
                new Product { ProductId = "LC-007", CategoryId = 2, Name = "Apple Mac mini M6", Price = 13999.00m, Stock = 12, Specs = "16GB Unified Memory, 512GB SSD, Ultra-compact desktop", ImageUrl = "images/mac-mini-m6.webp" },

                // 3. Tablets
                new Product { ProductId = "TB-001", CategoryId = 3, Name = "Apple iPad Air 13 M4", Price = 14499.00m, Stock = 14, Specs = "13-inch Liquid Retina, Apple Pencil Pro support", ImageUrl = "images/ipad-air-m4.jpg" },
                new Product { ProductId = "TB-002", CategoryId = 3, Name = "Apple iPad Pro 13-inch", Price = 27999.00m, Stock = 7, Specs = "256GB, Tandem OLED display, M4 architecture", ImageUrl = "images/ipad-pro-13.jpeg" },
                new Product { ProductId = "TB-003", CategoryId = 3, Name = "Samsung Galaxy Tab S12 Ultra", Price = 26499.00m, Stock = 6, Specs = "14.6-inch Dynamic AMOLED 2X, S-Pen included", ImageUrl = "images/tab-s12-ultra.avif" },
                new Product { ProductId = "TB-004", CategoryId = 3, Name = "Samsung Galaxy Tab S12+", Price = 20999.00m, Stock = 9, Specs = "12.4-inch 120Hz screen, 512GB storage, S-Pen", ImageUrl = "images/tab-s12-plus.webp" },
                new Product { ProductId = "TB-005", CategoryId = 3, Name = "Xiaomi Pad 9 Pro", Price = 11999.00m, Stock = 15, Specs = "144Hz 3K display, Quad stereo speakers, Snapdragon", ImageUrl = "images/xiaomi-pad-9-pro.jpeg" },
                new Product { ProductId = "TB-006", CategoryId = 3, Name = "OnePlus Pad 3", Price = 10499.00m, Stock = 12, Specs = "7:5 ReadFit display ratio, 67W SUPERVOOC charging", ImageUrl = "images/oneplus-pad-3.jpg" },

                // 4. Headphones and Earphones
                new Product { ProductId = "AU-001", CategoryId = 4, Name = "Apple AirPods 5", Price = 3999.00m, Stock = 30, Specs = "Active Noise Cancellation, Adaptive Audio, USB-C case", ImageUrl = "images/airpods-5.jpg" },
                new Product { ProductId = "AU-002", CategoryId = 4, Name = "Apple AirPods Max 2", Price = 12499.00m, Stock = 10, Specs = "Lossless USB-C audio, Personalized Spatial Audio", ImageUrl = "images/apple-airpods-max-2.png" },
                new Product { ProductId = "AU-003", CategoryId = 4, Name = "Sony WH-1000XM6", Price = 8499.00m, Stock = 16, Specs = "Industry-leading ANC, 30-hour battery, LDAC Hi-Res", ImageUrl = "images/sony-wh1000xm6.webp" },
                new Product { ProductId = "AU-004", CategoryId = 4, Name = "JBL Live 780NC", Price = 3499.00m, Stock = 25, Specs = "True Adaptive Noise Cancelling, 65-hour playtime", ImageUrl = "images/jbl-live-780nc.webp" },
                new Product { ProductId = "AU-005", CategoryId = 4, Name = "Bose QuietComfort Ultra", Price = 8999.00m, Stock = 12, Specs = "Bose Immersive Audio, World-class active noise cancellation", ImageUrl = "images/bose-qc-ultra.jpg" },
                new Product { ProductId = "AU-006", CategoryId = 4, Name = "Samsung Galaxy Buds3 Pro", Price = 3799.00m, Stock = 28, Specs = "Blade design, 24-bit Hi-Fi sound, Galaxy AI noise control", ImageUrl = "images/galaxy-buds3-pro.jpg" },

                // 5. Smartwatches
                new Product { ProductId = "WT-001", CategoryId = 5, Name = "Apple Watch Ultra 3", Price = 19999.00m, Stock = 8, Specs = "49mm Titanium case, Dual-frequency GPS, 72h battery", ImageUrl = "images/apple-watch-ultra-3.jpg" },
                new Product { ProductId = "WT-002", CategoryId = 5, Name = "Apple Watch Series 11", Price = 10499.00m, Stock = 18, Specs = "Thinner design, Sleep apnea alerts, Fast charging", ImageUrl = "images/apple-watch-series-11.png" },
                new Product { ProductId = "WT-003", CategoryId = 5, Name = "Samsung Galaxy Watch 8 Classic", Price = 8999.00m, Stock = 15, Specs = "Physical rotating bezel, BioActive Sensor, Sapphire crystal", ImageUrl = "images/galaxy-watch-8-classic.jpg" },
                new Product { ProductId = "WT-004", CategoryId = 5, Name = "Huawei Watch GT 6 Pro", Price = 7499.00m, Stock = 14, Specs = "Titanium alloy build, 14-day battery life, ECG analysis", ImageUrl = "images/huawei-gt6-pro.jpg" },
                new Product { ProductId = "WT-005", CategoryId = 5, Name = "Huawei Watch Ultimate 2", Price = 15999.00m, Stock = 6, Specs = "Zirconium liquid metal case, 100m diving certification", ImageUrl = "images/huawei-watch-ultimate-2.jpeg" },
                new Product { ProductId = "WT-006", CategoryId = 5, Name = "Google Pixel Watch 5", Price = 7999.00m, Stock = 12, Specs = "Actua display, Advanced Fitbit biometric tracking engine", ImageUrl = "images/pixel-watch-5.webp" },

                // 6. Speakers and Sound Systems
                new Product { ProductId = "SPK-001", CategoryId = 6, Name = "JBL Xtreme 5", Price = 6499.00m, Stock = 15, Specs = "IP67 waterproof, AI sound boost, replaceable battery", ImageUrl = "images/jbl-xtreme-5.jpg" },
                new Product { ProductId = "SPK-002", CategoryId = 6, Name = "JBL Go 5", Price = 899.00m, Stock = 40, Specs = "Ultra-compact, 7h playtime, integrated carabiner strap", ImageUrl = "images/jbl-go-5.jpg" },
                new Product { ProductId = "SPK-003", CategoryId = 6, Name = "JBL PartyBox 330", Price = 11999.00m, Stock = 8, Specs = "240W output, dynamic synchronized lightshow, mic inputs", ImageUrl = "images/jbl-partybox-330.webp" },
                new Product { ProductId = "SPK-004", CategoryId = 6, Name = "Samsung HW-Q990F Soundbar", Price = 21999.00m, Stock = 6, Specs = "11.1.4 Channel Dolby Atmos, Wireless Q-Symphony", ImageUrl = "images/samsung-hw-q990f.jpg" },
                new Product { ProductId = "SPK-005", CategoryId = 6, Name = "Sony BRAVIA Theatre System 6", Price = 18499.00m, Stock = 5, Specs = "360 Spatial Sound Mapping, Wireless rear sub package", ImageUrl = "images/sony-theatre-6.webp" },
                new Product { ProductId = "SPK-006", CategoryId = 6, Name = "Bose Lifestyle Ultra System", Price = 32999.00m, Stock = 4, Specs = "OmniJewel satellite speakers, Acoustimass wireless bass", ImageUrl = "images/bose-lifestyle-ultra.png" },

                // 7. Televisions
                new Product { ProductId = "TV-001", CategoryId = 7, Name = "Samsung 65\" S95H OLED TV", Price = 39999.00m, Stock = 5, Specs = "Glare-free QD-OLED panel, 144Hz 4K gaming, NQ4 AI Gen3", ImageUrl = "images/samsung-s95h-oled.jpeg" },
                new Product { ProductId = "TV-002", CategoryId = 7, Name = "LG 65\" C6 OLED TV", Price = 36499.00m, Stock = 6, Specs = "Alpha 10 AI processor, 4x HDMI 2.1, G-Sync / FreeSync", ImageUrl = "images/lg-c6-oled.jpg" },
                new Product { ProductId = "TV-003", CategoryId = 7, Name = "Hisense 65\" U7N Mini-LED TV", Price = 16999.00m, Stock = 10, Specs = "144Hz Game Mode Pro, Full array local dimming, HDR10+", ImageUrl = "images/hisense-u7n.jpg" },
                new Product { ProductId = "TV-004", CategoryId = 7, Name = "Sony 65\" BRAVIA 8 OLED TV", Price = 37999.00m, Stock = 4, Specs = "Acoustic Surface Audio+, XR Processor, PS5 auto HDR", ImageUrl = "images/sony-bravia-8.webp" },
                new Product { ProductId = "TV-005", CategoryId = 7, Name = "TCL 65\" C855 Premium QD-Mini LED", Price = 18999.00m, Stock = 8, Specs = "3500 nits peak brightness, ONKYO 2.2.2 Hi-Fi audio", ImageUrl = "images/tcl-c855.jpeg" },

                // 8. Gaming
                new Product { ProductId = "GM-001", CategoryId = 8, Name = "Sony PlayStation 5 Pro", Price = 16499.00m, Stock = 10, Specs = "2TB SSD, Advanced Ray Tracing, PSSR 4K 60FPS upscaling", ImageUrl = "images/ps5-pro.webp" },
                new Product { ProductId = "GM-002", CategoryId = 8, Name = "Microsoft Xbox Series X", Price = 13499.00m, Stock = 12, Specs = "True 4K 120FPS, Quick Resume, Xbox Velocity Architecture", ImageUrl = "images/xbox-series-x.jpg" },
                new Product { ProductId = "GM-003", CategoryId = 8, Name = "Nintendo Switch 2", Price = 8999.00m, Stock = 15, Specs = "8-inch 1080p display, DLSS dock mode, Joy-Con 2 controllers", ImageUrl = "images/switch-2.webp" },
                new Product { ProductId = "GM-004", CategoryId = 8, Name = "Meta Quest 3 (512GB)", Price = 14999.00m, Stock = 9, Specs = "High-res color passthrough, Snapdragon XR2 Gen 2 VR/MR", ImageUrl = "images/meta-quest-3.webp" },
                new Product { ProductId = "GM-005", CategoryId = 8, Name = "Logitech G Pro X Superlight 2", Price = 2799.00m, Stock = 25, Specs = "LIGHTFORCE hybrid optical switches, 60g ultralight, HERO 2", ImageUrl = "images/gpro-x-superlight-2.jpg" },

                // 9. Cameras and Drones
                new Product { ProductId = "CD-001", CategoryId = 9, Name = "Sony A7R VI Mirrorless Camera", Price = 69999.00m, Stock = 3, Specs = "61MP Exmor R sensor, AI-assisted autofocus, 8K recording", ImageUrl = "images/sony-a7r-vi.jpg" },
                new Product { ProductId = "CD-002", CategoryId = 9, Name = "DJI Osmo Pocket 4", Price = 9499.00m, Stock = 12, Specs = "1-inch CMOS sensor, 3-axis gimbal, 4K 120FPS OLED", ImageUrl = "images/dji-pocket-4.jpg" },
                new Product { ProductId = "CD-003", CategoryId = 9, Name = "GoPro Mission 1 Pro", Price = 8499.00m, Stock = 15, Specs = "5.3K 60FPS, HyperSmooth stabilization, 10m waterproof", ImageUrl = "images/gopro-mission-1.jpeg" },
                new Product { ProductId = "CD-004", CategoryId = 9, Name = "DJI Mini 5 Pro Drone", Price = 17999.00m, Stock = 8, Specs = "Sub-249g lightweight, Omnidirectional obstacle sensing", ImageUrl = "images/dji-mini-5-pro.jpeg" },
                new Product { ProductId = "CD-005", CategoryId = 9, Name = "DJI Avata 360 FPV Drone", Price = 22499.00m, Stock = 5, Specs = "360-degree immersive video capture, Goggles 3 bundle", ImageUrl = "images/dji-avata-360.webp" },
                new Product { ProductId = "CD-006", CategoryId = 9, Name = "Insta360 Link 2 4K Webcam", Price = 4299.00m, Stock = 14, Specs = "AI tracking gimbal, 1/2-inch sensor, gesture controls", ImageUrl = "images/insta360-link-2.jpeg" },

                // 10. 3D Printers
                new Product { ProductId = "PRN-001", CategoryId = 10, Name = "Bambu Lab P2S Combo", Price = 19999.00m, Stock = 6, Specs = "Multi-color AMS printing, enclosed chamber, 500mm/s speed", ImageUrl = "images/bambu-p2s.png" },
                new Product { ProductId = "PRN-002", CategoryId = 10, Name = "Bambu Lab H-series", Price = 26499.00m, Stock = 4, Specs = "High-temperature engineering filament support, active heat", ImageUrl = "images/bambu-h-series.jpg" },
                new Product { ProductId = "PRN-003", CategoryId = 10, Name = "Bambu Lab X1 Carbon", Price = 28999.00m, Stock = 5, Specs = "Micro Lidar leveling, Dual AI cameras, Carbon fiber body", ImageUrl = "images/bambu-x1-carbon.jpg" },
                new Product { ProductId = "PRN-004", CategoryId = 10, Name = "Bambu Lab A1 Combo", Price = 11499.00m, Stock = 10, Specs = "AMS Lite multi-color printing, active motor noise canceling", ImageUrl = "images/bambu-a1-combo.png" },
                new Product { ProductId = "PRN-005", CategoryId = 10, Name = "Creality K2 Pro", Price = 17499.00m, Stock = 7, Specs = "600mm/s CoreXY, CFS multi-color system, AI camera", ImageUrl = "images/creality-k2-pro.webp" },
                new Product { ProductId = "PRN-006", CategoryId = 10, Name = "Creality K2 Plus", Price = 21999.00m, Stock = 5, Specs = "Large 350x350x350mm build volume, heated chamber", ImageUrl = "images/creality-k2-plus.jpg" },
                new Product { ProductId = "PRN-007", CategoryId = 10, Name = "Creality Ender-5 Max", Price = 12999.00m, Stock = 8, Specs = "Cube frame stability, 300x300x400mm print volume", ImageUrl = "images/creality-ender-5-max.jpg" },
                new Product { ProductId = "PRN-008", CategoryId = 10, Name = "Creality K1C High-Speed", Price = 11999.00m, Stock = 12, Specs = "Carbon fiber filament nozzle, 600mm/s max print speed", ImageUrl = "images/creality-k1c.webp" },

                // 11. Accessories and Other Electronics
                new Product { ProductId = "AC-001", CategoryId = 11, Name = "Anker Prime 20,000mAh Power Bank", Price = 2299.00m, Stock = 25, Specs = "200W total output, smart digital display, dual USB-C PD", ImageUrl = "images/anker-prime-pb.jpg" },
                new Product { ProductId = "AC-002", CategoryId = 11, Name = "UGREEN Nexode 20,000mAh Power Bank", Price = 1699.00m, Stock = 22, Specs = "130W fast charge, TFT color screen, multi-device charge", ImageUrl = "images/ugreen-nexode-pb.jpg" },
                new Product { ProductId = "AC-003", CategoryId = 11, Name = "Logitech MX Keys S Keyboard", Price = 2199.00m, Stock = 20, Specs = "Smart illumination, Perfect Stroke keys, multi-device pair", ImageUrl = "images/logitech-mx-keys-s.jpg" },
                new Product { ProductId = "AC-004", CategoryId = 11, Name = "Razer BlackWidow V4 Pro Keyboard", Price = 4499.00m, Stock = 12, Specs = "Mechanical Green switches, Command Dial, Chroma RGB", ImageUrl = "images/razer-blackwidow-v4.jpg" },
                new Product { ProductId = "AC-005", CategoryId = 11, Name = "ASUS RT-BE96U Wi-Fi 7 Router", Price = 12999.00m, Stock = 6, Specs = "Quad-band Wi-Fi 7, 19,000 Mbps bandwidth, dual 10G ports", ImageUrl = "images/asus-rt-be96u.jpg" },
                new Product { ProductId = "AC-006", CategoryId = 11, Name = "TP-Link Deco BE85 Mesh (2-Pack)", Price = 18499.00m, Stock = 5, Specs = "Whole-home Wi-Fi 7 mesh, 22 Gbps, 10G SFP+/Ethernet", ImageUrl = "images/tplink-deco-be85.jpg" },
                new Product { ProductId = "AC-007", CategoryId = 11, Name = "Epson EcoTank ET-4950 Printer", Price = 7999.00m, Stock = 9, Specs = "Cartridge-free ink tank, ADF scanner, Ethernet & Wi-Fi", ImageUrl = "images/epson-et-4950.jpg" },
                new Product { ProductId = "AC-008", CategoryId = 11, Name = "Canon MegaTank G3270 Printer", Price = 3999.00m, Stock = 14, Specs = "Refillable ink system, wireless printing, 3-in-1 print/copy", ImageUrl = "images/canon-g3270.jpg" },
                new Product { ProductId = "AC-009", CategoryId = 11, Name = "Creality Hyper PLA Filament (1kg)", Price = 399.00m, Stock = 50, Specs = "High-speed 600mm/s printing, 1.75mm diameter, tough", ImageUrl = "images/creality-pla-filament.jpg" },
                new Product { ProductId = "AC-010", CategoryId = 11, Name = "Bambu Lab PETG Basic Filament (1kg)", Price = 449.00m, Stock = 45, Specs = "Water-resistant, high impact resistance, smooth finish", ImageUrl = "images/bambu-petg-filament.jpg" },
                new Product { ProductId = "AC-011", CategoryId = 11, Name = "eSUN ABS+ 3D Filament (1kg)", Price = 429.00m, Stock = 35, Specs = "High strength and heat resistance, modified ABS formula", ImageUrl = "images/esun-abs-filament.jpg" },
                new Product { ProductId = "AC-012", CategoryId = 11, Name = "Overture TPU Flexible Filament (1kg)", Price = 549.00m, Stock = 30, Specs = "95A shore hardness, elastic, high-wear resistance", ImageUrl = "images/overture-tpu-filament.jpg" },
                new Product { ProductId = "AC-013", CategoryId = 11, Name = "Anycubic Standard UV Resin (1kg)", Price = 599.00m, Stock = 25, Specs = "405nm LCD photopolymer resin, high precision, low odor", ImageUrl = "images/anycubic-uv-resin.jpg" },
                new Product { ProductId = "AC-014", CategoryId = 11, Name = "Bambu Lab Carbon-Fiber PLA (1kg)", Price = 699.00m, Stock = 20, Specs = "Carbon fiber reinforced, high stiffness, matte finish", ImageUrl = "images/bambu-cf-filament.jpg" },
                new Product { ProductId = "AC-015", CategoryId = 11, Name = "Samsung T9 Portable SSD (2TB)", Price = 3999.00m, Stock = 18, Specs = "USB 3.2 Gen 2x2, up to 2000MB/s read/write speed", ImageUrl = "images/samsung-t9-ssd.jpg" },
                new Product { ProductId = "AC-016", CategoryId = 11, Name = "SanDisk Extreme Portable SSD (2TB)", Price = 3499.00m, Stock = 20, Specs = "IP65 water/dust resistance, drop protection, 1050MB/s", ImageUrl = "images/sandisk-extreme-ssd.jpg" }
            );
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database seeding notice: {ex.Message}");
    }
}

app.Run();