/**
 * BINARY TECH HUB - PRODUCT CATALOG DATA
 * Course: INFS 221 (Frontend Web Development)
 * Mirrors: MySQL database 'binary_tech_hub.products'
 */

const API_BASE_URL = 'http://localhost:5185/api';

var categories = [
  { id: 1, name: "Smartphones", slug: "smartphones" },
  { id: 2, name: "Laptops and Computers", slug: "laptops-and-computers" },
  { id: 3, name: "Tablets", slug: "tablets" },
  { id: 4, name: "Headphones and Earphones", slug: "headphones-and-earphones" },
  { id: 5, name: "Smartwatches", slug: "smartwatches" },
  { id: 6, name: "Speakers and Sound Systems", slug: "speakers-and-sound-systems" },
  { id: 7, name: "Televisions", slug: "televisions" },
  { id: 8, name: "Gaming", slug: "gaming" },
  { id: 9, name: "Cameras and Drones", slug: "cameras-and-drones" },
  { id: 10, name: "3D Printers", slug: "3d-printers" },
  { id: 11, name: "Accessories and Other Electronics", slug: "accessories-and-other-electronics" }
];

// Seeded static fallback catalog (used if API is offline)
var fallbackProducts = [
  // 1. Smartphones
  {
    id: "SP-001",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Apple iPhone 18 Pro",
    price: 27999.00,
    stock: 12,
    specs: "256GB, A20 Pro chip, ProMotion OLED display",
    image: "images/iphone-18-pro.webp"
  },
  {
    id: "SP-002",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Apple iPhone 18 Pro Max",
    price: 31999.00,
    stock: 10,
    specs: "512GB, Periscope telephoto, Titanium frame",
    image: "images/iphone-18-promax.webp"
  },
  {
    id: "SP-003",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Samsung Galaxy S26",
    price: 21499.00,
    stock: 15,
    specs: "256GB, Dynamic AMOLED 2X, Snapdragon Gen 5",
    image: "images/galaxy-s26.jpg"
  },
  {
    id: "SP-004",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Samsung Galaxy S26+",
    price: 24999.00,
    stock: 12,
    specs: "512GB, 4900mAh battery, 45W fast charge",
    image: "images/galaxy-s26-plus.jpg"
  },
  {
    id: "SP-005",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Samsung Galaxy S26 Ultra",
    price: 30499.00,
    stock: 8,
    specs: "512GB, Embedded S-Pen, 200MP Quad Camera",
    image: "images/galaxy-s26-ultra.jpg"
  },
  {
    id: "SP-006",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Samsung Galaxy S26 FE",
    price: 15999.00,
    stock: 20,
    specs: "128GB, Flagship-tier camera, 120Hz display",
    image: "images/galaxy-s26-fe.avif"
  },
  {
    id: "SP-007",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Google Pixel 11",
    price: 18499.00,
    stock: 14,
    specs: "128GB, Tensor G6 chip, Next-gen Google AI",
    image: "images/pixel-11.webp"
  },
  {
    id: "SP-008",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Google Pixel 11 Pro",
    price: 23999.00,
    stock: 9,
    specs: "256GB, Super Actua display, Pro camera controls",
    image: "images/pixel-11-pro.webp"
  },
  {
    id: "SP-009",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Xiaomi 17",
    price: 16999.00,
    stock: 16,
    specs: "256GB, Leica optical lens, 90W HyperCharge",
    image: "images/xiaomi-17.webp"
  },
  {
    id: "SP-010",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Xiaomi 17 Ultra",
    price: 26499.00,
    stock: 7,
    specs: "512GB, 1-inch sensor, Variable aperture",
    image: "images/xiaomi-17-ultra.jpeg"
  },
  {
    id: "SP-011",
    categoryId: 1,
    categoryName: "Smartphones",
    name: "Huawei Nova 14",
    price: 11499.00,
    stock: 18,
    specs: "256GB, 60MP portrait front cam, SuperCharge",
    image: "images/huawei-nova-14.jpeg"
  },

  // 2. Laptops and Computers
  {
    id: "LC-001",
    categoryId: 2,
    categoryName: "Laptops and Computers",
    name: "Apple MacBook Neo",
    price: 17999.00,
    stock: 14,
    specs: "13-inch, Apple Silicon, Fanless ultra-portable design",
    image: "images/macbook-neo.jpg"
  },
  {
    id: "LC-002",
    categoryId: 2,
    categoryName: "Laptops and Computers",
    name: "Apple MacBook Air M5",
    price: 22999.00,
    stock: 15,
    specs: "16GB RAM, 512GB SSD, Liquid Retina display",
    image: "images/macbook_air_m5.png"
  },
  {
    id: "LC-003",
    categoryId: 2,
    categoryName: "Laptops and Computers",
    name: "Apple MacBook Pro 16 M5 Pro",
    price: 38999.00,
    stock: 8,
    specs: "24GB Unified Memory, 1TB SSD, 120Hz XDR",
    image: "images/macbook-pro-m5-pro.webp"
  },
  {
    id: "LC-004",
    categoryId: 2,
    categoryName: "Laptops and Computers",
    name: "Apple MacBook Pro 16 M5 Max",
    price: 54999.00,
    stock: 5,
    specs: "36GB Unified Memory, 1TB SSD, Extreme GPU power",
    image: "images/macbook-pro-m5-max.png"
  },
  {
    id: "LC-005",
    categoryId: 2,
    categoryName: "Laptops and Computers",
    name: "ASUS Zenbook Duo (2026)",
    price: 34999.00,
    stock: 6,
    specs: "Dual 14-inch OLED touch displays, Intel Core Ultra 9",
    image: "images/zenbook-duo-2026.jpg"
  },
  {
    id: "LC-006",
    categoryId: 2,
    categoryName: "Laptops and Computers",
    name: "MSI Claw 8 EX AI+",
    price: 16999.00,
    stock: 11,
    specs: "8-inch 120Hz FHD, 32GB LPDDR5x, 80Wh battery",
    image: "images/msi-claw-8.jpg"
  },
  {
    id: "LC-007",
    categoryId: 2,
    categoryName: "Laptops and Computers",
    name: "Apple Mac mini M6",
    price: 13999.00,
    stock: 12,
    specs: "16GB Unified Memory, 512GB SSD, Ultra-compact desktop",
    image: "images/mac-mini-m6.webp"
  },

  // 3. Tablets
  {
    id: "TB-001",
    categoryId: 3,
    categoryName: "Tablets",
    name: "Apple iPad Air 13 M4",
    price: 14499.00,
    stock: 14,
    specs: "13-inch Liquid Retina, Apple Pencil Pro support",
    image: "images/ipad-air-m4.jpg"
  },
  {
    id: "TB-002",
    categoryId: 3,
    categoryName: "Tablets",
    name: "Apple iPad Pro 13-inch",
    price: 27999.00,
    stock: 7,
    specs: "256GB, Tandem OLED display, M4 architecture",
    image: "images/ipad-pro-13.jpeg"
  },
  {
    id: "TB-003",
    categoryId: 3,
    categoryName: "Tablets",
    name: "Samsung Galaxy Tab S12 Ultra",
    price: 26499.00,
    stock: 6,
    specs: "14.6-inch Dynamic AMOLED 2X, S-Pen included",
    image: "images/tab-s12-ultra.avif"
  },
  {
    id: "TB-004",
    categoryId: 3,
    categoryName: "Tablets",
    name: "Samsung Galaxy Tab S12+",
    price: 20999.00,
    stock: 9,
    specs: "12.4-inch 120Hz screen, 512GB storage, S-Pen",
    image: "images/tab-s12-plus.webp"
  },
  {
    id: "TB-005",
    categoryId: 3,
    categoryName: "Tablets",
    name: "Xiaomi Pad 9 Pro",
    price: 11999.00,
    stock: 15,
    specs: "144Hz 3K display, Quad stereo speakers, Snapdragon",
    image: "images/xiaomi-pad-9-pro.jpeg"
  },
  {
    id: "TB-006",
    categoryId: 3,
    categoryName: "Tablets",
    name: "OnePlus Pad 3",
    price: 10499.00,
    stock: 12,
    specs: "7:5 ReadFit display ratio, 67W SUPERVOOC charging",
    image: "images/oneplus-pad-3.jpg"
  },

  // 4. Headphones and Earphones
  {
    id: "AU-001",
    categoryId: 4,
    categoryName: "Headphones and Earphones",
    name: "Apple AirPods 5",
    price: 3999.00,
    stock: 30,
    specs: "Active Noise Cancellation, Adaptive Audio, USB-C case",
    image: "images/airpods-5.jpg"
  },
  {
    id: "AU-002",
    categoryId: 4,
    categoryName: "Headphones and Earphones",
    name: "Apple AirPods Max 2",
    price: 12499.00,
    stock: 10,
    specs: "Lossless USB-C audio, Personalized Spatial Audio",
    image: "images/apple-airpods-max-2.png"
  },
  {
    id: "AU-003",
    categoryId: 4,
    categoryName: "Headphones and Earphones",
    name: "Sony WH-1000XM6",
    price: 8499.00,
    stock: 16,
    specs: "Industry-leading ANC, 30-hour battery, LDAC Hi-Res",
    image: "images/sony-wh1000xm6.webp"
  },
  {
    id: "AU-004",
    categoryId: 4,
    categoryName: "Headphones and Earphones",
    name: "JBL Live 780NC",
    price: 3499.00,
    stock: 25,
    specs: "True Adaptive Noise Cancelling, 65-hour playtime",
    image: "images/jbl-live-780nc.webp"
  },
  {
    id: "AU-005",
    categoryId: 4,
    categoryName: "Headphones and Earphones",
    name: "Bose QuietComfort Ultra",
    price: 8999.00,
    stock: 12,
    specs: "Bose Immersive Audio, World-class active noise cancellation",
    image: "images/bose-qc-ultra.jpg"
  },
  {
    id: "AU-006",
    categoryId: 4,
    categoryName: "Headphones and Earphones",
    name: "Samsung Galaxy Buds3 Pro",
    price: 3799.00,
    stock: 28,
    specs: "Blade design, 24-bit Hi-Fi sound, Galaxy AI noise control",
    image: "images/galaxy-buds3-pro.jpg"
  },

  // 5. Smartwatches
  {
    id: "WT-001",
    categoryId: 5,
    categoryName: "Smartwatches",
    name: "Apple Watch Ultra 3",
    price: 19999.00,
    stock: 8,
    specs: "49mm Titanium case, Dual-frequency GPS, 72h battery",
    image: "images/apple-watch-ultra-3.jpg"
  },
  {
    id: "WT-002",
    categoryId: 5,
    categoryName: "Smartwatches",
    name: "Apple Watch Series 11",
    price: 10499.00,
    stock: 18,
    specs: "Thinner design, Sleep apnea alerts, Fast charging",
    image: "images/apple-watch-series-11.png"
  },
  {
    id: "WT-003",
    categoryId: 5,
    categoryName: "Smartwatches",
    name: "Samsung Galaxy Watch 8 Classic",
    price: 8999.00,
    stock: 15,
    specs: "Physical rotating bezel, BioActive Sensor, Sapphire crystal",
    image: "images/galaxy-watch-8-classic.jpg"
  },
  {
    id: "WT-004",
    categoryId: 5,
    categoryName: "Smartwatches",
    name: "Huawei Watch GT 6 Pro",
    price: 7499.00,
    stock: 14,
    specs: "Titanium alloy build, 14-day battery life, ECG analysis",
    image: "images/huawei-gt6-pro.jpg"
  },
  {
    id: "WT-005",
    categoryId: 5,
    categoryName: "Smartwatches",
    name: "Huawei Watch Ultimate 2",
    price: 15999.00,
    stock: 6,
    specs: "Zirconium liquid metal case, 100m diving certification",
    image: "images/huawei-watch-ultimate-2.jpeg"
  },
  {
    id: "WT-006",
    categoryId: 5,
    categoryName: "Smartwatches",
    name: "Google Pixel Watch 5",
    price: 7999.00,
    stock: 12,
    specs: "Actua display, Advanced Fitbit biometric tracking engine",
    image: "images/pixel-watch-5.webp"
  },

  // 6. Speakers and Sound Systems
  {
    id: "SPK-001",
    categoryId: 6,
    categoryName: "Speakers and Sound Systems",
    name: "JBL Xtreme 5",
    price: 6499.00,
    stock: 15,
    specs: "IP67 waterproof, AI sound boost, replaceable battery",
    image: "images/jbl-xtreme-5.jpg"
  },
  {
    id: "SPK-002",
    categoryId: 6,
    categoryName: "Speakers and Sound Systems",
    name: "JBL Go 5",
    price: 899.00,
    stock: 40,
    specs: "Ultra-compact, 7h playtime, integrated carabiner strap",
    image: "images/jbl-go-5.jpg"
  },
  {
    id: "SPK-003",
    categoryId: 6,
    categoryName: "Speakers and Sound Systems",
    name: "JBL PartyBox 330",
    price: 11999.00,
    stock: 8,
    specs: "240W output, dynamic synchronized lightshow, mic inputs",
    image: "images/jbl-partybox-330.webp"
  },
  {
    id: "SPK-004",
    categoryId: 6,
    categoryName: "Speakers and Sound Systems",
    name: "Samsung HW-Q990F Soundbar",
    price: 21999.00,
    stock: 6,
    specs: "11.1.4 Channel Dolby Atmos, Wireless Q-Symphony",
    image: "images/samsung-hw-q990f.jpg"
  },
  {
    id: "SPK-005",
    categoryId: 6,
    categoryName: "Speakers and Sound Systems",
    name: "Sony BRAVIA Theatre System 6",
    price: 18499.00,
    stock: 5,
    specs: "360 Spatial Sound Mapping, Wireless rear sub package",
    image: "images/sony-theatre-6.webp"
  },
  {
    id: "SPK-006",
    categoryId: 6,
    categoryName: "Speakers and Sound Systems",
    name: "Bose Lifestyle Ultra System",
    price: 32999.00,
    stock: 4,
    specs: "OmniJewel satellite speakers, Acoustimass wireless bass",
    image: "images/bose-lifestyle-ultra.png"
  },

  // 7. Televisions
  {
    id: "TV-001",
    categoryId: 7,
    categoryName: "Televisions",
    name: 'Samsung 65" S95H OLED TV',
    price: 39999.00,
    stock: 5,
    specs: "Glare-free QD-OLED panel, 144Hz 4K gaming, NQ4 AI Gen3",
    image: "images/samsung-s95h-oled.jpeg"
  },
  {
    id: "TV-002",
    categoryId: 7,
    categoryName: "Televisions",
    name: 'LG 65" C6 OLED TV',
    price: 36499.00,
    stock: 6,
    specs: "Alpha 10 AI processor, 4x HDMI 2.1, G-Sync / FreeSync",
    image: "images/lg-c6-oled.jpg"
  },
  {
    id: "TV-003",
    categoryId: 7,
    categoryName: "Televisions",
    name: 'Hisense 65" U7N Mini-LED TV',
    price: 16999.00,
    stock: 10,
    specs: "144Hz Game Mode Pro, Full array local dimming, HDR10+",
    image: "images/hisense-u7n.jpg"
  },
  {
    id: "TV-004",
    categoryId: 7,
    categoryName: "Televisions",
    name: 'Sony 65" BRAVIA 8 OLED TV',
    price: 37999.00,
    stock: 4,
    specs: "Acoustic Surface Audio+, XR Processor, PS5 auto HDR",
    image: "images/sony-bravia-8.webp"
  },
  {
    id: "TV-005",
    categoryId: 7,
    categoryName: "Televisions",
    name: 'TCL 65" C855 Premium QD-Mini LED',
    price: 18999.00,
    stock: 8,
    specs: "3500 nits peak brightness, ONKYO 2.2.2 Hi-Fi audio",
    image: "images/tcl-c855.jpeg"
  },

  // 8. Gaming
  {
    id: "GM-001",
    categoryId: 8,
    categoryName: "Gaming",
    name: "Sony PlayStation 5 Pro",
    price: 16499.00,
    stock: 10,
    specs: "2TB SSD, Advanced Ray Tracing, PSSR 4K 60FPS upscaling",
    image: "images/ps5-pro.webp"
  },
  {
    id: "GM-002",
    categoryId: 8,
    categoryName: "Gaming",
    name: "Microsoft Xbox Series X",
    price: 13499.00,
    stock: 12,
    specs: "True 4K 120FPS, Quick Resume, Xbox Velocity Architecture",
    image: "images/xbox-series-x.jpg"
  },
  {
    id: "GM-003",
    categoryId: 8,
    categoryName: "Gaming",
    name: "Nintendo Switch 2",
    price: 8999.00,
    stock: 15,
    specs: "8-inch 1080p display, DLSS dock mode, Joy-Con 2 controllers",
    image: "images/switch-2.webp"
  },
  {
    id: "GM-004",
    categoryId: 8,
    categoryName: "Gaming",
    name: "Meta Quest 3 (512GB)",
    price: 14999.00,
    stock: 9,
    specs: "High-res color passthrough, Snapdragon XR2 Gen 2 VR/MR",
    image: "images/meta-quest-3.webp"
  },
  {
    id: "GM-005",
    categoryId: 8,
    categoryName: "Gaming",
    name: "Logitech G Pro X Superlight 2",
    price: 2799.00,
    stock: 25,
    specs: "LIGHTFORCE hybrid optical switches, 60g ultralight, HERO 2",
    image: "images/gpro-x-superlight-2.jpg"
  },

  // 9. Cameras and Drones
  {
    id: "CD-001",
    categoryId: 9,
    categoryName: "Cameras and Drones",
    name: "Sony A7R VI Mirrorless Camera",
    price: 69999.00,
    stock: 3,
    specs: "61MP Exmor R sensor, AI-assisted autofocus, 8K recording",
    image: "images/sony-a7r-vi.jpg"
  },
  {
    id: "CD-002",
    categoryId: 9,
    categoryName: "Cameras and Drones",
    name: "DJI Osmo Pocket 4",
    price: 9499.00,
    stock: 12,
    specs: "1-inch CMOS sensor, 3-axis gimbal, 4K 120FPS OLED",
    image: "images/dji-pocket-4.jpg"
  },
  {
    id: "CD-003",
    categoryId: 9,
    categoryName: "Cameras and Drones",
    name: "GoPro Mission 1 Pro",
    price: 8499.00,
    stock: 15,
    specs: "5.3K 60FPS, HyperSmooth stabilization, 10m waterproof",
    image: "images/gopro-mission-1.jpeg"
  },
  {
    id: "CD-004",
    categoryId: 9,
    categoryName: "Cameras and Drones",
    name: "DJI Mini 5 Pro Drone",
    price: 17999.00,
    stock: 8,
    specs: "Sub-249g lightweight, Omnidirectional obstacle sensing",
    image: "images/dji-mini-5-pro.jpeg"
  },
  {
    id: "CD-005",
    categoryId: 9,
    categoryName: "Cameras and Drones",
    name: "DJI Avata 360 FPV Drone",
    price: 22499.00,
    stock: 5,
    specs: "360-degree immersive video capture, Goggles 3 bundle",
    image: "images/dji-avata-360.webp"
  },
  {
    id: "CD-006",
    categoryId: 9,
    categoryName: "Cameras and Drones",
    name: "Insta360 Link 2 4K Webcam",
    price: 4299.00,
    stock: 14,
    specs: "AI tracking gimbal, 1/2-inch sensor, gesture controls",
    image: "images/insta360-link-2.jpeg"
  },

  // 10. 3D Printers
  {
    id: "PRN-001",
    categoryId: 10,
    categoryName: "3D Printers",
    name: "Bambu Lab P2S Combo",
    price: 19999.00,
    stock: 6,
    specs: "Multi-color AMS printing, enclosed chamber, 500mm/s speed",
    image: "images/bambu-p2s.png"
  },
  {
    id: "PRN-002",
    categoryId: 10,
    categoryName: "3D Printers",
    name: "Bambu Lab H-series",
    price: 26499.00,
    stock: 4,
    specs: "High-temperature engineering filament support, active heat",
    image: "images/bambu-h-series.jpg"
  },
  {
    id: "PRN-003",
    categoryId: 10,
    categoryName: "3D Printers",
    name: "Bambu Lab X1 Carbon",
    price: 28999.00,
    stock: 5,
    specs: "Micro Lidar leveling, Dual AI cameras, Carbon fiber body",
    image: "images/bambu-x1-carbon.jpg"
  },
  {
    id: "PRN-004",
    categoryId: 10,
    categoryName: "3D Printers",
    name: "Bambu Lab A1 Combo",
    price: 11499.00,
    stock: 10,
    specs: "AMS Lite multi-color printing, active motor noise canceling",
    image: "images/bambu-a1-combo.png"
  },
  {
    id: "PRN-005",
    categoryId: 10,
    categoryName: "3D Printers",
    name: "Creality K2 Pro",
    price: 17499.00,
    stock: 7,
    specs: "600mm/s CoreXY, CFS multi-color system, AI camera",
    image: "images/creality-k2-pro.webp"
  },
  {
    id: "PRN-006",
    categoryId: 10,
    categoryName: "3D Printers",
    name: "Creality K2 Plus",
    price: 21999.00,
    stock: 5,
    specs: "Large 350x350x350mm build volume, heated chamber",
    image: "images/creality-k2-plus.jpg"
  },
  {
    id: "PRN-007",
    categoryId: 10,
    categoryName: "3D Printers",
    name: "Creality Ender-5 Max",
    price: 12999.00,
    stock: 8,
    specs: "Cube frame stability, 300x300x400mm print volume",
    image: "images/creality-ender-5-max.jpg"
  },
  {
    id: "PRN-008",
    categoryId: 10,
    categoryName: "3D Printers",
    name: "Creality K1C High-Speed",
    price: 11999.00,
    stock: 12,
    specs: "Carbon fiber filament nozzle, 600mm/s max print speed",
    image: "images/creality-k1c.webp"
  },

  // 11. Accessories and Other Electronics
  {
    id: "AC-001",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Anker Prime 20,000mAh Power Bank",
    price: 2299.00,
    stock: 25,
    specs: "200W total output, smart digital display, dual USB-C PD",
    image: "images/anker-prime-pb.jpg"
  },
  {
    id: "AC-002",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "UGREEN Nexode 20,000mAh Power Bank",
    price: 1699.00,
    stock: 22,
    specs: "130W fast charge, TFT color screen, multi-device charge",
    image: "images/ugreen-nexode-pb.jpg"
  },
  {
    id: "AC-003",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Logitech MX Keys S Keyboard",
    price: 2199.00,
    stock: 20,
    specs: "Smart illumination, Perfect Stroke keys, multi-device pair",
    image: "images/logitech-mx-keys-s.jpg"
  },
  {
    id: "AC-004",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Razer BlackWidow V4 Pro Keyboard",
    price: 4499.00,
    stock: 12,
    specs: "Mechanical Green switches, Command Dial, Chroma RGB",
    image: "images/razer-blackwidow-v4.jpg"
  },
  {
    id: "AC-005",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "ASUS RT-BE96U Wi-Fi 7 Router",
    price: 12999.00,
    stock: 6,
    specs: "Quad-band Wi-Fi 7, 19,000 Mbps bandwidth, dual 10G ports",
    image: "images/asus-rt-be96u.jpg"
  },
  {
    id: "AC-006",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "TP-Link Deco BE85 Mesh (2-Pack)",
    price: 18499.00,
    stock: 5,
    specs: "Whole-home Wi-Fi 7 mesh, 22 Gbps, 10G SFP+/Ethernet",
    image: "images/tplink-deco-be85.jpg"
  },
  {
    id: "AC-007",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Epson EcoTank ET-4950 Printer",
    price: 7999.00,
    stock: 9,
    specs: "Cartridge-free ink tank, ADF scanner, Ethernet & Wi-Fi",
    image: "images/epson-et-4950.jpg"
  },
  {
    id: "AC-008",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Canon MegaTank G3270 Printer",
    price: 3999.00,
    stock: 14,
    specs: "Refillable ink system, wireless printing, 3-in-1 print/copy",
    image: "images/canon-g3270.jpg"
  },
  {
    id: "AC-009",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Creality Hyper PLA Filament (1kg)",
    price: 399.00,
    stock: 50,
    specs: "High-speed 600mm/s printing, 1.75mm diameter, tough",
    image: "images/creality-pla-filament.jpg"
  },
  {
    id: "AC-010",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Bambu Lab PETG Basic Filament (1kg)",
    price: 449.00,
    stock: 45,
    specs: "Water-resistant, high impact resistance, smooth finish",
    image: "images/bambu-petg-filament.jpg"
  },
  {
    id: "AC-011",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "eSUN ABS+ 3D Filament (1kg)",
    price: 429.00,
    stock: 35,
    specs: "High strength and heat resistance, modified ABS formula",
    image: "images/esun-abs-filament.jpg"
  },
  {
    id: "AC-012",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Overture TPU Flexible Filament (1kg)",
    price: 549.00,
    stock: 30,
    specs: "95A shore hardness, elastic, high-wear resistance",
    image: "images/overture-tpu-filament.jpg"
  },
  {
    id: "AC-013",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Anycubic Standard UV Resin (1kg)",
    price: 599.00,
    stock: 25,
    specs: "405nm LCD photopolymer resin, high precision, low odor",
    image: "images/anycubic-uv-resin.jpg"
  },
  {
    id: "AC-014",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Bambu Lab Carbon-Fiber PLA (1kg)",
    price: 699.00,
    stock: 20,
    specs: "Carbon fiber reinforced, high stiffness, matte finish",
    image: "images/bambu-cf-filament.jpg"
  },
  {
    id: "AC-015",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "Samsung T9 Portable SSD (2TB)",
    price: 3999.00,
    stock: 18,
    specs: "USB 3.2 Gen 2x2, up to 2000MB/s read/write speed",
    image: "images/samsung-t9-ssd.jpg"
  },
  {
    id: "AC-016",
    categoryId: 11,
    categoryName: "Accessories and Other Electronics",
    name: "SanDisk Extreme Portable SSD (2TB)",
    price: 3499.00,
    stock: 20,
    specs: "IP65 water/dust resistance, drop protection, 1050MB/s",
    image: "images/sandisk-extreme-ssd.jpg"
  }
];

// Active products array used across all views
var products = typeof fallbackProducts !== "undefined" ? [...fallbackProducts] : [];

// Fetch live products from MySQL via ASP.NET Core API
async function fetchProductsFromApi() {
  try {
    const response = await fetch(`${API_BASE_URL}/products`);
    if (!response.ok) {
      throw new Error(`API returned status ${response.status}`);
    }

    const liveData = await response.json();

    if (Array.isArray(liveData) && liveData.length > 0) {
      products = liveData.map(item => {
        const cat = categories.find(c => c.id === item.categoryId);
        return {
          id: item.productId || item.id,
          productId: item.productId || item.id,
          categoryId: item.categoryId,
          categoryName: cat ? cat.name : "Electronics",
          name: item.name,
          price: Number(item.price),
          stock: item.stock,
          specs: item.specs || "",
          image: item.imageUrl || item.image || "images/placeholder.jpg",
          imageUrl: item.imageUrl || item.image || "images/placeholder.jpg"
        };
      });

      window.dispatchEvent(new CustomEvent('bth:products-loaded', { detail: products }));
    }
  } catch (err) {
    console.warn("Could not fetch live products from API, using fallback data:", err);
  }
}

fetchProductsFromApi();