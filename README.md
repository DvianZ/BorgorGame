# 🍔 Burger Cooking Game (Unity 6.6 Compatible)

Proyek ini adalah rebuild arsitektur memasak ala *Undercooked / Overcooked* yang disesuaikan menjadi **Game Memasak Burger 2D** yang siap langsung dimainkan (*playable out of the box*).

Asset visual saat ini menggunakan representasi **kotak-kotak warna & sprite 2D sederhana** dengan feedback interaksi penuh (animasi chopping, progress bar memasak, visual tumpukan burger di atas piring, dan HUD pesanan dinamis).

---

## 🚀 Cara Membuka di Unity 6.6

1. Buka **Unity Hub**.
2. Klik tombol **Add** (atau *Add project from disk*).
3. Pilih folder: `d:\eksperimen\Unity\gameNihKyanya`.
4. Pastikan editor yang terpilih adalah **Unity 6 (6000.6.0f1)**.
5. Buka project, lalu buka scene di `Assets/Scenes/MainKitchen.unity` (atau langsung klik tombol **Play ▶**).
6. Game langsung otomatis menginisialisasi dapur, stasiun, pemain, dan sistem pesanan!

---

## 🎮 Kontrol Permainan

| Tombol | Fungsi |
|---|---|
| **W, A, S, D** / **Tombol Panah** | Menggerakkan Chef (Pemain) |
| **Spasi** atau **E** | **Interaksi Utama**: Ambil bahan dari dispenser / Taruh ke meja / Susun ke piring / Antar ke kasir |
| **F** atau **C** | **Aksi Kerja**: Memotong (Chop) selada / tomat di Meja Pemotong |
| **Q** | Membuang bahan yang sedang dipegang ke tempat sampah |
| **R** | Restart ronde permainan |

---

## 🍳 Stasiun Dapur (Kitchen Stations)

1. **Dispenser Roti Bawah (Cokelat Muda)**: Menyediakan Roti Bawah (*Bun Bottom*).
2. **Dispenser Daging Mentah (Merah)**: Menyediakan Daging Mentah (*Raw Patty*).
3. **Panggangan / Kompor (Hitam/Oranye)**:
   - Taruh daging mentah di sini, otomatis memasak (5 detik) menjadi Daging Matang (*Cooked Patty*).
   - Awas: jika dibiarkan terlalu lama (>6 detik), daging akan **GOSONG** (*Burnt*)!
4. **Meja Pemotong (Kayu)**:
   - Taruh Selada (*Lettuce*) atau Tomat (*Tomato*).
   - Dekati meja dan tekan **F** berkali-kali untuk memotong hingga selesai.
5. **Dispenser Selada & Tomat (Hijau & Merah Terang)**: Mengambil sayuran segar.
6. **Dispenser Keju (Kuning)**: Mengambil lembaran keju cheddar.
7. **Dispenser Roti Atas (Cokelat Keemasan)**: Menyediakan Roti Atas penutup burger (*Bun Top*).
8. **Dispenser Piring (Putih/Perak)**: Mengambil piring untuk merangkai pesanan.
9. **Meja Prep (Clear Counter)**: Tempat menaruh bahan atau merangkai burger.
10. **Delivery Counter (Biru)**: Antarkan piring berisi burger yang sudah lengkap ke sini untuk mendapatkan skor dan uang!
11. **Tempat Sampah (Merah/X)**: Buang bahan yang salah atau daging yang gosong.

---

## 📋 Resep Burger

1. **Burger Biasa ($30)**:
   - `Roti Bawah` + `Daging Matang` + `Roti Atas`
2. **Cheeseburger ($40)**:
   - `Roti Bawah` + `Daging Matang` + `Keju` + `Roti Atas`
3. **Burger Sayur ($45)**:
   - `Roti Bawah` + `Selada Potong` + `Tomat Iris` + `Keju` + `Roti Atas`
4. **Burger Spesial Lengkap ($60)**:
   - `Roti Bawah` + `Daging Matang` + `Keju` + `Selada Potong` + `Tomat Iris` + `Roti Atas`

---

## 🧩 Struktur Arsitektur Kode (`Assets/Scripts/`)

- `Core/ItemType.cs`: Definisi enum bahan & database warna/skala.
- `Core/OrderManager.cs`: Antrian pesanan acak, timer countdown, dan logika matching resep.
- `Core/KitchenBootstrap.cs`: Master setup yang otomatis menyusun layout dapur, kamera, dan pemain.
- `Cooking/KitchenItem.cs`: Representasi fisik bahan + sistem penumpukan (*stacking*) di atas piring.
- `Cooking/BaseCounter.cs`: Kelas induk untuk semua meja counter.
- `Cooking/ClearCounter.cs`: Meja persiapan serbaguna.
- `Cooking/DispenserCounter.cs`: Wadah bahan tanpa batas.
- `Cooking/CuttingCounter.cs`: Meja pemotong dengan progress bar.
- `Cooking/StoveCounter.cs`: Kompor/panggangan otomatis dengan sistem gosong.
- `Cooking/DeliveryCounter.cs`: Jendela penyerahan pesanan.
- `Cooking/TrashCounter.cs`: Tempat pembuangan.
- `Player/PlayerController.cs`: Kontrol gerakan 2D top-down dan deteksi interaksi counter.
- `UI/GameHUD.cs`: Antarmuka layar (kartu pesanan aktif, timer, skor, panduan tombol).
