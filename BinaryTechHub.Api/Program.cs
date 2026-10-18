using Microsoft.EntityFrameworkCore;
using BinaryTechHub.Api.Data;
using BinaryTechHub.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

// 2. Database context
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

// 4. Auto-create schema and seed full product catalog into Aiven MySQL
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            // Smartphones
            new Product { Id = "SP-001", CategoryId = 1, CategoryName = "Smartphones", Name = "Apple iPhone 18 Pro", Price = 27999.00m, Stock = 12, Specs = "256GB, A20 Pro chip, ProMotion OLED display", Image = "images/iphone-18-pro.webp" },
            new Product { Id = "SP-002", CategoryId = 1, CategoryName = "Smartphones", Name = "Apple iPhone 18 Pro Max", Price = 31999.00m, Stock = 10, Specs = "512GB, Periscope telephoto, Titanium frame", Image = "images/iphone-18-promax.webp" },
            new Product { Id = "SP-003", CategoryId = 1, CategoryName = "Smartphones", Name = "Samsung Galaxy S26", Price = 21499.00m, Stock = 15, Specs = "256GB, Dynamic AMOLED 2X, Snapdragon Gen 5", Image = "images/galaxy-s26.jpg" },
            new Product { Id = "SP-004", CategoryId = 1, CategoryName = "Smartphones", Name = "Samsung Galaxy S26+", Price = 24999.00m, Stock = 12, Specs = "512GB, 4900mAh battery, 45W fast charge", Image = "images/galaxy-s26-plus.jpg" },
            new Product { Id = "SP-005", CategoryId = 1, CategoryName = "Smartphones", Name = "Samsung Galaxy S26 Ultra", Price = 30499.00m, Stock = 8, Specs = "512GB, Embedded S-Pen, 200MP Quad Camera", Image = "images/galaxy-s26-ultra.jpg" },
            new Product { Id = "SP-006", CategoryId = 1, CategoryName = "Smartphones", Name = "Samsung Galaxy S26 FE", Price = 15999.00m, Stock = 20, Specs = "128GB, Flagship-tier camera, 120Hz display", Image = "images/galaxy-s26-fe.avif" },
            new Product { Id = "SP-007", CategoryId = 1, CategoryName = "Smartphones", Name = "Google Pixel 11", Price = 18499.00m, Stock = 14, Specs = "128GB, Tensor G6 chip, Next-gen Google AI", Image = "images/pixel-11.webp" },
            new Product { Id = "SP-008", CategoryId = 1, CategoryName = "Smartphones", Name = "Google Pixel 11 Pro", Price = 23999.00m, Stock = 9, Specs = "256GB, Super Actua display, Pro camera controls", Image = "images/pixel-11-pro.webp" },
            new Product { Id = "SP-009", CategoryId = 1, CategoryName = "Smartphones", Name = "Xiaomi 17", Price = 16999.00m, Stock = 16, Specs = "256GB, Leica optical lens, 90W HyperCharge", Image = "images/xiaomi-17.webp" },
            new Product { Id = "SP-010", CategoryId = 1, CategoryName = "Smartphones", Name = "Xiaomi 17 Ultra", Price = 26499.00m, Stock = 7, Specs = "512GB, 1-inch sensor, Variable aperture", Image = "images/xiaomi-17-ultra.jpeg" },
            new Product { Id = "SP-011", CategoryId = 1, CategoryName = "Smartphones", Name = "Huawei Nova 14", Price = 11499.00m, Stock = 18, Specs = "256GB, 60MP portrait front cam, SuperCharge", Image = "images/huawei-nova-14.jpeg" },

            // Laptops and Computers
            new Product { Id = "LC-001", CategoryId = 2, CategoryName = "Laptops and Computers", Name = "Apple MacBook Neo", Price = 17999.00m, Stock = 14, Specs = "13-inch, Apple Silicon, Fanless ultra-portable design", Image = "images/macbook-neo.jpg" },
            new Product { Id = "LC-002", CategoryId = 2, CategoryName = "Laptops and Computers", Name = "Apple MacBook Air M5", Price = 22999.00m, Stock = 15, Specs = "16GB RAM, 512GB SSD, Liquid Retina display", Image = "images/macbook_air_m5.png" },
            new Product { Id = "LC-003", CategoryId = 2, CategoryName = "Laptops and Computers", Name = "Apple MacBook Pro 16 M5 Pro", Price = 38999.00m, Stock = 8, Specs = "24GB Unified Memory, 1TB SSD, 120Hz XDR", Image = "images/macbook-pro-m5-pro.webp" },
            new Product { Id = "LC-004", CategoryId = 2, CategoryName = "Laptops and Computers", Name = "Apple MacBook Pro 16 M5 Max", Price = 54999.00m, Stock = 5, Specs = "36GB Unified Memory, 1TB SSD, Extreme GPU power", Image = "images/macbook-pro-m5-max.png" },
            new Product { Id = "LC-005", CategoryId = 2, CategoryName = "Laptops and Computers", Name = "ASUS Zenbook Duo (2026)", Price = 34999.00m, Stock = 6, Specs = "Dual 14-inch OLED touch displays, Intel Core Ultra 9", Image = "images/zenbook-duo-2026.jpg" },
            new Product { Id = "LC-006", CategoryId = 2, CategoryName = "Laptops and Computers", Name = "MSI Claw 8 EX AI+", Price = 16999.00m, Stock = 11, Specs = "8-inch 120Hz FHD, 32GB LPDDR5x, 80Wh battery", Image = "images/msi-claw-8.jpg" },
            new Product { Id = "LC-007", CategoryId = 2, CategoryName = "Laptops and Computers", Name = "Apple Mac mini M6", Price = 13999.00m, Stock = 12, Specs = "16GB Unified Memory, 512GB SSD, Ultra-compact desktop", Image = "images/mac-mini-m6.webp" },

            // Tablets
            new Product { Id = "TB-001", CategoryId = 3, CategoryName = "Tablets", Name = "Apple iPad Air 13 M4", Price = 14499.00m, Stock = 14, Specs = "13-inch Liquid Retina, Apple Pencil Pro support", Image = "images/ipad-air-m4.jpg" },
            new Product { Id = "TB-002", CategoryId = 3, CategoryName = "Tablets", Name = "Apple iPad Pro 13-inch", Price = 27999.00m, Stock = 7, Specs = "256GB, Tandem OLED display, M4 architecture", Image = "images/ipad-pro-13.jpeg" },
            new Product { Id = "TB-003", CategoryId = 3, CategoryName = "Tablets", Name = "Samsung Galaxy Tab S12 Ultra", Price = 26499.00m, Stock = 6, Specs = "14.6-inch Dynamic AMOLED 2X, S-Pen included", Image = "images/tab-s12-ultra.avif" },
            new Product { Id = "TB-004", CategoryId = 3, CategoryName = "Tablets", Name = "Samsung Galaxy Tab S12+", Price = 20999.00m, Stock = 9, Specs = "12.4-inch 120Hz screen, 512GB storage, S-Pen", Image = "images/tab-s12-plus.webp" },
            new Product { Id = "TB-005", CategoryId = 3, CategoryName = "Tablets", Name = "Xiaomi Pad 9 Pro", Price = 11999.00m, Stock = 15, Specs = "144Hz 3K display, Quad stereo speakers, Snapdragon", Image = "images/xiaomi-pad-9-pro.jpeg" },
            new Product { Id = "TB-006", CategoryId = 3, CategoryName = "Tablets", Name = "OnePlus Pad 3", Price = 10499.00m, Stock = 12, Specs = "7:5 ReadFit display ratio, 67W SUPERVOOC charging", Image = "images/oneplus-pad-3.jpg" },

            // Headphones and Earphones
            new Product { Id = "AU-001", CategoryId = 4, CategoryName = "Headphones and Earphones", Name = "Apple AirPods 5", Price = 3999.00m, Stock = 30, Specs = "Active Noise Cancellation, Adaptive Audio, USB-C case", Image = "images/airpods-5.jpg" },
            new Product { Id = "AU-002", CategoryId = 4, CategoryName = "Headphones and Earphones", Name = "Apple AirPods Max 2", Price = 12499.00m, Stock = 10, Specs = "Lossless USB-C audio, Personalized Spatial Audio", Image = "images/apple-airpods-max-2.png" },
            new Product { Id = "AU-003", CategoryId = 4, CategoryName = "Headphones and Earphones", Name = "Sony WH-1000XM6", Price = 8499.00m, Stock = 16, Specs = "Industry-leading ANC, 30-hour battery, LDAC Hi-Res", Image = "images/sony-wh1000xm6.webp" },
            new Product { Id = "AU-004", CategoryId = 4, CategoryName = "Headphones and Earphones", Name = "JBL Live 780NC", Price = 3499.00m, Stock = 25, Specs = "True Adaptive Noise Cancelling, 65-hour playtime", Image = "images/jbl-live-780nc.webp" },
            new Product { Id = "AU-005", CategoryId = 4, CategoryName = "Headphones and Earphones", Name = "Bose QuietComfort Ultra", Price = 8999.00m, Stock = 12, Specs = "Bose Immersive Audio, World-class active noise cancellation", Image = "images/bose-qc-ultra.jpg" },
            new Product { Id = "AU-006", CategoryId = 4, CategoryName = "Headphones and Earphones", Name = "Samsung Galaxy Buds3 Pro", Price = 3799.00m, Stock = 28, Specs = "Blade design, 24-bit Hi-Fi sound, Galaxy AI noise control", Image = "images/galaxy-buds3-pro.jpg" },

            // Smartwatches
            new Product { Id = "WT-001", CategoryId = 5, CategoryName = "Smartwatches", Name = "Apple Watch Ultra 3", Price = 19999.00m, Stock = 8, Specs = "49mm Titanium case, Dual-frequency GPS, 72h battery", Image = "images/apple-watch-ultra-3.jpg" },
            new Product { Id = "WT-002", CategoryId = 5, CategoryName = "Smartwatches", Name = "Apple Watch Series 11", Price = 10499.00m, Stock = 18, Specs = "Thinner design, Sleep apnea alerts, Fast charging", Image = "images/apple-watch-series-11.png" },
            new Product { Id = "WT-003", CategoryId = 5, CategoryName = "Smartwatches", Name = "Samsung Galaxy Watch 8 Classic", Price = 8999.00m, Stock = 15, Specs = "Physical rotating bezel, BioActive Sensor, Sapphire crystal", Image = "images/galaxy-watch-8-classic.jpg" },
            new Product { Id = "WT-004", CategoryId = 5, CategoryName = "Smartwatches", Name = "Huawei Watch GT 6 Pro", Price = 7499.00m, Stock = 14, Specs = "Titanium alloy build, 14-day battery life, ECG analysis", Image = "images/huawei-gt6-pro.jpg" },
            new Product { Id = "WT-005", CategoryId = 5, CategoryName = "Smartwatches", Name = "Huawei Watch Ultimate 2", Price = 15999.00m, Stock = 6, Specs = "Zirconium liquid metal case, 100m diving certification", Image = "images/huawei-watch-ultimate-2.jpeg" },
            new Product { Id = "WT-006", CategoryId = 5, CategoryName = "Smartwatches", Name = "Google Pixel Watch 5", Price = 7999.00m, Stock = 12, Specs = "Actua display, Advanced Fitbit biometric tracking engine", Image = "images/pixel-watch-5.webp" },

            // Speakers and Sound Systems
            new Product { Id = "SPK-001", CategoryId = 6, CategoryName = "Speakers and Sound Systems", Name = "JBL Xtreme 5", Price = 6499.00m, Stock = 15, Specs = "IP67 waterproof, AI sound boost, replaceable battery", Image = "images/jbl-xtreme-5.jpg" },
            new Product { Id = "SPK-002", CategoryId = 6, CategoryName = "Speakers and Sound Systems", Name = "JBL Go 5", Price = 899.00m, Stock = 40, Specs = "Ultra-compact, 7h playtime, integrated carabiner strap", Image = "images/jbl-go-5.jpg" },
            new Product { Id = "SPK-003", CategoryId = 6, CategoryName = "Speakers and Sound Systems", Name = "JBL PartyBox 330", Price = 11999.00m, Stock = 8, Specs = "240W output, dynamic synchronized lightshow, mic inputs", Image = "images/jbl-partybox-330.webp" },
            new Product { Id = "SPK-004", CategoryId = 6, CategoryName = "Speakers and Sound Systems", Name = "Samsung HW-Q990F Soundbar", Price = 21999.00m, Stock = 6, Specs = "11.1.4 Channel Dolby Atmos, Wireless Q-Symphony", Image = "images/samsung-hw-q990f.jpg" },
            new Product { Id = "SPK-005", CategoryId = 6, CategoryName = "Speakers and Sound Systems", Name = "Sony BRAVIA Theatre System 6", Price = 18499.00m, Stock = 5, Specs = "360 Spatial Sound Mapping, Wireless rear sub package", Image = "images/sony-theatre-6.webp" },
            new Product { Id = "SPK-006", CategoryId = 6, CategoryName = "Speakers and Sound Systems", Name = "Bose Lifestyle Ultra System", Price = 32999.00m, Stock = 4, Specs = "OmniJewel satellite speakers, Acoustimass wireless bass", Image = "images/bose-lifestyle-ultra.png" },

            // Televisions
            new Product { Id = "TV-001", CategoryId = 7, CategoryName = "Televisions", Name = "Samsung 65\" S95H OLED TV", Price = 39999.00m, Stock = 5, Specs = "Glare-free QD-OLED panel, 144Hz 4K gaming, NQ4 AI Gen3", Image = "images/samsung-s95h-oled.jpeg" },
            new Product { Id = "TV-002", CategoryId = 7, CategoryName = "Televisions", Name = "LG 65\" C6 OLED TV", Price = 36499.00m, Stock = 6, Specs = "Alpha 10 AI processor, 4x HDMI 2.1, G-Sync / FreeSync", Image = "images/lg-c6-oled.jpg" },
            new Product { Id = "TV-003", CategoryId = 7, CategoryName = "Televisions", Name = "Hisense 65\" U7N Mini-LED TV", Price = 16999.00m, Stock = 10, Specs = "144Hz Game Mode Pro, Full array local dimming, HDR10+", Image = "images/hisense-u7n.jpg" },
            new Product { Id = "TV-004", CategoryId = 7, CategoryName = "Televisions", Name = "Sony 65\" BRAVIA 8 OLED TV", Price = 37999.00m, Stock = 4, Specs = "Acoustic Surface Audio+, XR Processor, PS5 auto HDR", Image = "images/sony-bravia-8.webp" },
            new Product { Id = "TV-005", CategoryId = 7, CategoryName = "Televisions", Name = "TCL 65\" C855 Premium QD-Mini LED", Price = 18999.00m, Stock = 8, Specs = "3500 nits peak brightness, ONKYO 2.2.2 Hi-Fi audio", Image = "images/tcl-c855.jpeg" },

            // Gaming
            new Product { Id = "GM-001", CategoryId = 8, CategoryName = "Gaming", Name = "Sony PlayStation 5 Pro", Price = 16499.00m, Stock = 10, Specs = "2TB SSD, Advanced Ray Tracing, PSSR 4K 60FPS upscaling", Image = "images/ps5-pro.webp" },
            new Product { Id = "GM-002", CategoryId = 8, CategoryName = "Gaming", Name = "Microsoft Xbox Series X", Price = 13499.00m, Stock = 12, Specs = "True 4K 120FPS, Quick Resume, Xbox Velocity Architecture", Image = "images/xbox-series-x.jpg" },
            new Product { Id = "GM-003", CategoryId = 8, CategoryName = "Gaming", Name = "Nintendo Switch 2", Price = 8999.00m, Stock = 15, Specs = "8-inch 1080p display, DLSS dock mode, Joy-Con 2 controllers", Image = "images/switch-2.webp" },
            new Product { Id = "GM-004", CategoryId = 8, CategoryName = "Gaming", Name = "Meta Quest 3 (512GB)", Price = 14999.00m, Stock = 9, Specs = "High-res color passthrough, Snapdragon XR2 Gen 2 VR/MR", Image = "images/meta-quest-3.webp" },
            new Product { Id = "GM-005", CategoryId = 8, CategoryName = "Gaming", Name = "Logitech G Pro X Superlight 2", Price = 2799.00m, Stock = 25, Specs = "LIGHTFORCE hybrid optical switches, 60g ultralight, HERO 2", Image = "images/gpro-x-superlight-2.jpg" },

            // Cameras and Drones
            new Product { Id = "CD-001", CategoryId = 9, CategoryName = "Cameras and Drones", Name = "Sony A7R VI Mirrorless Camera", Price = 69999.00m, Stock = 3, Specs = "61MP Exmor R sensor, AI-assisted autofocus, 8K recording", Image = "images/sony-a7r-vi.jpg" },
            new Product { Id = "CD-002", CategoryId = 9, CategoryName = "Cameras and Drones", Name = "DJI Osmo Pocket 4", Price = 9499.00m, Stock = 12, Specs = "1-inch CMOS sensor, 3-axis gimbal, 4K 120FPS OLED", Image = "images/dji-pocket-4.jpg" },
            new Product { Id = "CD-003", CategoryId = 9, CategoryName = "Cameras and Drones", Name = "GoPro Mission 1 Pro", Price = 8499.00m, Stock = 15, Specs = "5.3K 60FPS, HyperSmooth stabilization, 10m waterproof", Image = "images/gopro-mission-1.jpeg" },
            new Product { Id = "CD-004", CategoryId = 9, CategoryName = "Cameras and Drones", Name = "DJI Mini 5 Pro Drone", Price = 17999.00m, Stock = 8, Specs = "Sub-249g lightweight, Omnidirectional obstacle sensing", Image = "images/dji-mini-5-pro.jpeg" },
            new Product { Id = "CD-005", CategoryId = 9, CategoryName = "Cameras and Drones", Name = "DJI Avata 360 FPV Drone", Price = 22499.00m, Stock = 5, Specs = "360-degree immersive video capture, Goggles 3 bundle", Image = "images/dji-avata-360.webp" },
            new Product { Id = "CD-006", CategoryId = 9, CategoryName = "Cameras and Drones", Name = "Insta360 Link 2 4K Webcam", Price = 4299.00m, Stock = 14, Specs = "AI tracking gimbal, 1/2-inch sensor, gesture controls", Image = "images/insta360-link-2.jpeg" },

            // 3D Printers
            new Product { Id = "PRN-001", CategoryId = 10, CategoryName = "3D Printers", Name = "Bambu Lab P2S Combo", Price = 19999.00m, Stock = 6, Specs = "Multi-color AMS printing, enclosed chamber, 500mm/s speed", Image = "images/bambu-p2s.png" },
            new Product { Id = "PRN-002", CategoryId = 10, CategoryName = "3D Printers", Name = "Bambu Lab H-series", Price = 26499.00m, Stock = 4, Specs = "High-temperature engineering filament support, active heat", Image = "images/bambu-h-series.jpg" },
            new Product { Id = "PRN-003", CategoryId = 10, CategoryName = "3D Printers", Name = "Bambu Lab X1 Carbon", Price = 28999.00m, Stock = 5, Specs = "Micro Lidar leveling, Dual AI cameras, Carbon fiber body", Image = "images/bambu-x1-carbon.jpg" },
            new Product { Id = "PRN-004", CategoryId = 10, CategoryName = "3D Printers", Name = "Bambu Lab A1 Combo", Price = 11499.00m, Stock = 10, Specs = "AMS Lite multi-color printing, active motor noise canceling", Image = "images/bambu-a1-combo.png" },
            new Product { Id = "PRN-005", CategoryId = 10, CategoryName = "3D Printers", Name = "Creality K2 Pro", Price = 17499.00m, Stock = 7, Specs = "600mm/s CoreXY, CFS multi-color system, AI camera", Image = "images/creality-k2-pro.webp" },
            new Product { Id = "PRN-006", CategoryId = 10, CategoryName = "3D Printers", Name = "Creality K2 Plus", Price = 21999.00m, Stock = 5, Specs = "Large 350x350x350mm build volume, heated chamber", Image = "images/creality-k2-plus.jpg" },
            new Product { Id = "PRN-007", CategoryId = 10, CategoryName = "3D Printers", Name = "Creality Ender-5 Max", Price = 12999.00m, Stock = 8, Specs = "Cube frame stability, 300x300x400mm print volume", Image = "images/creality-ender-5-max.jpg" },
            new Product { Id = "PRN-008", CategoryId = 10, CategoryName = "3D Printers", Name = "Creality K1C High-Speed", Price = 11999.00m, Stock = 12, Specs = "Carbon fiber filament nozzle, 600mm/s max print speed", Image = "images/creality-k1c.webp" },

            // Accessories and Other Electronics
            new Product { Id = "AC-001", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Anker Prime 20,000mAh Power Bank", Price = 2299.00m, Stock = 25, Specs = "200W total output, smart digital display, dual USB-C PD", Image = "images/anker-prime-pb.jpg" },
            new Product { Id = "AC-002", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "UGREEN Nexode 20,000mAh Power Bank", Price = 1699.00m, Stock = 22, Specs = "130W fast charge, TFT color screen, multi-device charge", Image = "images/ugreen-nexode-pb.jpg" },
            new Product { Id = "AC-003", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Logitech MX Keys S Keyboard", Price = 2199.00m, Stock = 20, Specs = "Smart illumination, Perfect Stroke keys, multi-device pair", Image = "images/logitech-mx-keys-s.jpg" },
            new Product { Id = "AC-004", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Razer BlackWidow V4 Pro Keyboard", Price = 4499.00m, Stock = 12, Specs = "Mechanical Green switches, Command Dial, Chroma RGB", Image = "images/razer-blackwidow-v4.jpg" },
            new Product { Id = "AC-005", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "ASUS RT-BE96U Wi-Fi 7 Router", Price = 12999.00m, Stock = 6, Specs = "Quad-band Wi-Fi 7, 19,000 Mbps bandwidth, dual 10G ports", Image = "images/asus-rt-be96u.jpg" },
            new Product { Id = "AC-006", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "TP-Link Deco BE85 Mesh (2-Pack)", Price = 18499.00m, Stock = 5, Specs = "Whole-home Wi-Fi 7 mesh, 22 Gbps, 10G SFP+/Ethernet", Image = "images/tplink-deco-be85.jpg" },
            new Product { Id = "AC-007", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Epson EcoTank ET-4950 Printer", Price = 7999.00m, Stock = 9, Specs = "Cartridge-free ink tank, ADF scanner, Ethernet & Wi-Fi", Image = "images/epson-et-4950.jpg" },
            new Product { Id = "AC-008", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Canon MegaTank G3270 Printer", Price = 3999.00m, Stock = 14, Specs = "Refillable ink system, wireless printing, 3-in-1 print/copy", Image = "images/canon-g3270.jpg" },
            new Product { Id = "AC-009", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Creality Hyper PLA Filament (1kg)", Price = 399.00m, Stock = 50, Specs = "High-speed 600mm/s printing, 1.75mm diameter, tough", Image = "images/creality-pla-filament.jpg" },
            new Product { Id = "AC-010", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Bambu Lab PETG Basic Filament (1kg)", Price = 449.00m, Stock = 45, Specs = "Water-resistant, high impact resistance, smooth finish", Image = "images/bambu-petg-filament.jpg" },
            new Product { Id = "AC-011", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "eSUN ABS+ 3D Filament (1kg)", Price = 429.00m, Stock = 35, Specs = "High strength and heat resistance, modified ABS formula", Image = "images/esun-abs-filament.jpg" },
            new Product { Id = "AC-012", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Overture TPU Flexible Filament (1kg)", Price = 549.00m, Stock = 30, Specs = "95A shore hardness, elastic, high-wear resistance", Image = "images/overture-tpu-filament.jpg" },
            new Product { Id = "AC-013", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Anycubic Standard UV Resin (1kg)", Price = 599.00m, Stock = 25, Specs = "405nm LCD photopolymer resin, high precision, low odor", Image = "images/anycubic-uv-resin.jpg" },
            new Product { Id = "AC-014", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Bambu Lab Carbon-Fiber PLA (1kg)", Price = 699.00m, Stock = 20, Specs = "Carbon fiber reinforced, high stiffness, matte finish", Image = "images/bambu-cf-filament.jpg" },
            new Product { Id = "AC-015", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "Samsung T9 Portable SSD (2TB)", Price = 3999.00m, Stock = 18, Specs = "USB 3.2 Gen 2x2, up to 2000MB/s read/write speed", Image = "images/samsung-t9-ssd.jpg" },
            new Product { Id = "AC-016", CategoryId = 11, CategoryName = "Accessories and Other Electronics", Name = "SanDisk Extreme Portable SSD (2TB)", Price = 3499.00m, Stock = 20, Specs = "IP65 water/dust resistance, drop protection, 1050MB/s", Image = "images/sandisk-extreme-ssd.jpg" }
        );
        db.SaveChanges();
    }
}

app.Run();