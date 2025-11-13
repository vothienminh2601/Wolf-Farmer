Một game mô phỏng nông trại được phát triển trong Unity — nơi người chơi có thể **trồng cây, chăn nuôi, thu hoạch, mở rộng đất và quản lý tài nguyên**.
Link Video Demo: https://drive.google.com/file/d/1aA5BCE_Ov8Io9aFN2CIz2I8DVWDIygiU/view?usp=sharing


## 🧭 Tổng quan
Dữ liệu game (Seed, Fruit, Animal, v.v.) được quản lý thông qua **CSV** thay vì ScriptableObject, giúp **dễ bảo trì và chỉnh sửa cho Game Designer (GD)**.

📖 Hướng dẫn chơi cơ bản
🌱 1. Trồng cây
- Click vào 1 mảnh đất trống.
- Chọn loại hạt giống muốn trồng.
- Nếu có đủ số lượng hạt, cây sẽ bắt đầu được trồng.
- Cây có 3 giai đoạn:
- Seed → Grow → Mature (trưởng thành)
- Khi trưởng thành, cây sẽ tự ra quả theo thời gian.
- Sau khi đạt đủ số quả tối đa, cây sẽ héo.
  
🍓 2. Thu hoạch trái
- Có 2 cách thu hoạch:
- Click trái cây để thu từng quả.
- Nhấn nút thu hoạch trong UI plot để thu toàn bộ.
- Trái sau khi thu sẽ vào kho.
  
🐮 3. Chăn nuôi
- Click mảnh đất → chọn vật nuôi (VD: Bò).
- Vật nuôi sẽ tự động sinh sản phẩm theo thời gian (VD: sữa).
- Click vào để thu sữa, hoặc dùng worker tự động thu sau này.
  
🛍 4. Mua / Bán
- Vào Shop → Chọn tab Mua / Bán:
- Mua: Hạt giống (theo bội số 10) hoặc Vật nuôi.
- Bán: Trái cây, sữa,… đang có trong kho.
- Chọn số lượng → nhấn nút mua / bán.
  
⚒ 5. Mở rộng đất
- Khi đủ tiền, bạn có thể mở thêm mảnh đất mới.
  
🔧 6. Nâng cấp thiết bị
- Vào UI nâng cấp → Tốn 500 coin mỗi lần.
- Mỗi cấp giúp giảm thời gian tăng trưởng & sinh sản 10%.


- Trick Test nhanh (Dev Mode)
- Nhấn nút ⬆ / ⬇ (Arrow Key) ==> Tăng / giảm Time.timeScale để tăng tốc game
- Nhấn nút C => Tăng +100 coin ngay lập tức

## Tính năng đã hoàn thành
### 🌱 **Hệ thống trồng trọt (Cultivation System)**
- Mỗi plot có thể trồng 1 loại cây (SeedData).
- Cây phát triển theo nhiều giai đoạn (seed → grow → mature → dead).
- Khi đạt giai đoạn Mature:
  - Cây sẽ **tự sinh Fruit** theo chu kỳ .
  - Khi sinh đủ , cây sẽ héo.
- Có thể **thu hoạch toàn bộ** hoặc **từng quả riêng lẻ**.
- Hiệu suất trồng trọt phụ thuộc vào **Equipment Level** (+10% mỗi cấp).

### 🐄 **Hệ thống chăn nuôi (Animal System)**
- Mỗi Plot có thể chứa 1 loại vật nuôi (Cow, Chicken, ...).
- Vật nuôi sản xuất Product (như Milk, Egg) theo thời gian.
- Hỗ trợ lưu và khôi phục dữ liệu vật nuôi.

### 🏡 **FarmManager & BuilderManager**
- Khởi tạo farm 9 plot ban đầu:
  - 1 plot xây **nhà chính**.
  - 3 plot canh tác.
- Cho phép mở rộng thêm plot mới theo dạng **hình vuông dần đều**.
- Lưu trạng thái plot (PlotPurpose) để khôi phục khi load game.

### 💾 **Save / Load System (UserData)**
- Tự động lưu và khôi phục:
  - Resource (coin, seeds, products, animals,…) => Đã hoàn thiện
  - Farm (plot purpose, cây trồng, vật nuôi,…) => Chưa hoàn thiện
  - Equipment Level
- Cho phép **Restart Game**:
  - Xóa toàn bộ dữ liệu và load lại Scene sạch.

### 🏪 **Shop System**
- 2 tab chính: **Buy** và **Sell**
- Buy: hiển thị danh sách Seed và Animal, chọn số lượng (seed chỉ bán theo bội số 10).
- Sell: hiển thị danh sách sản phẩm (Fruit, Milk, Egg), chọn số lượng bán.
- Tính tổng giá trị giao dịch tự động.

### 🧺 **Resource System**
- Quản lý toàn bộ tài nguyên:
  - Coin, Seeds, Animals, Products, Equipment, Workers.
- Giao tiếp với UIResource để hiển thị:
  - Số coin, công nhân, plot đang dùng / trống, hạt giống, sản phẩm.

### 🧩 **Data-Driven qua CSV**
- `SeedCSVReader`, `FruitCSVReader`, `AnimalCSVReader` đọc dữ liệu từ file CSV.
- Quản lý bằng `DataManager`.
- Asset (sprite, prefab) được load qua **Addressables**.

### ⚙️ **Equipment System**
- Equipment có thể **nâng cấp level**.
- Mỗi level tăng 10% hiệu suất (giảm thời gian sinh trưởng cây và sản phẩm).
- Mỗi lần nâng cấp tốn 500 coin.


## Ghi chú & Hạn chế hiện tại
Phần chưa hoàn thiện
- Worker hiện vẫn chưa thể tự động trồng cây, thu hoạch, mới chỉ di chuyển được.
- Chưa animation cho cây, trái, worker, UI...
- Chưa polish UI / VFX / feedback, còn đơn giản.

- Khi tắt game và mở lại:
  + Chỉ mới load được tài nguyên (coin, seed, product).
  + Chưa load lại cây trồng, vật nuôi, trạng thái các plot.

- Một số lưu ý: Các thông số như giá bán, tốc độ tăng trưởng, thời gian sinh sản, số lần thu hoạch có thể điều chỉnh trực tiếp trong file CSV để cân bằng game.

🌱 Do hạn chế asset có sẵn, một số cây trồng không giống hoàn toàn với yêu cầu đề bài.


