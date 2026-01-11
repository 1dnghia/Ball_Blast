# Hướng dẫn sử dụng Path Movement System

## Tổng quan
Hệ thống cho phép obstacles di chuyển theo đường vẽ (path) thay vì rơi tự do như Level 1. Hệ thống này hoàn hảo cho Level 2+ trong game Ball Blast.

## Các bước setup

### 1. Tạo PathData Asset
1. Trong Unity, Right-click trong Project window
2. Chọn: `Create > Ball Blast > Path Data`
3. Đặt tên file, ví dụ: `Level2_PathData`

### 2. Vẽ Path
1. Chọn PathData asset vừa tạo
2. Trong Inspector, click nút **"Start Drawing Path"**
3. Chuyển sang Scene view
4. **Click** vào các vị trí bạn muốn tạo điểm path
   - Điểm đầu tiên (màu xanh lá) = điểm bắt đầu
   - Điểm cuối cùng (màu đỏ) = điểm kết thúc
   - Các điểm giữa (màu vàng) = các điểm dẫn đường
5. **Shift + Click** trên điểm để xóa điểm đó
6. Click **"Stop Drawing Path"** khi hoàn thành

### 3. Cấu hình PathData Settings
Trong Inspector của PathData:
- **Path Speed**: Tốc độ di chuyển trên đường (mặc định: 2)
- **Loop Path**: Obstacle sẽ quay lại đầu path khi đến cuối
- **Ping Pong**: Obstacle sẽ di chuyển qua lại trên path
- **Spawn Position**: Vị trí spawn trên path (0 = đầu, 1 = cuối)

### 4. Cấu hình Level Data
1. Mở LevelData asset cho Level 2
2. Thay đổi **Movement Type** thành `Path`
3. Kéo PathData vừa tạo vào field **Path Data**

### 5. Cập nhật Obstacle Prefab
1. Mở Obstacle prefab
2. Add component: **ObstaclePathMovement**
3. Lưu prefab

### 6. Visualize Path trong Scene (Tùy chọn)
1. Tạo Empty GameObject trong Scene
2. Add component: **PathVisualizer**
3. Kéo PathData vào field của PathVisualizer
4. Bạn sẽ thấy path hiển thị trong Scene view

## Cách hoạt động

### Normal Movement (Level 1)
- Movement Type = `Normal`
- Obstacles spawn từ hai bên màn hình
- Rơi xuống dưới theo trọng lực
- Di chuyển qua lại giữa hai biên

### Path Movement (Level 2+)
- Movement Type = `Path`
- Obstacles spawn tại điểm đầu tiên của path
- Di chuyển theo đường đã vẽ
- Có thể loop hoặc ping-pong

## Ví dụ Path cho Level 2

### Path hình sóng (Wave)
```
Point 1: (-8, 4)
Point 2: (-4, 5)
Point 3: (0, 4)
Point 4: (4, 5)
Point 5: (8, 4)
```
Settings:
- Loop Path: ✓
- Ping Pong: ✗
- Path Speed: 3

### Path hình tròn
```
Vẽ 8-10 điểm tạo thành hình tròn
```
Settings:
- Loop Path: ✓
- Ping Pong: ✗
- Path Speed: 2

### Path qua lại ngang
```
Point 1: (-6, 3)
Point 2: (6, 3)
```
Settings:
- Loop Path: ✗
- Ping Pong: ✓
- Path Speed: 4

## Tips
1. **Vẽ path từ trái sang phải** để dễ quản lý
2. **Càng nhiều điểm** = đường đi càng mượt, nhưng tốn performance hơn
3. **Path Speed** nên điều chỉnh dựa trên độ dài path
4. Dùng **PathVisualizer** để kiểm tra path trong khi chơi game
5. Có thể tạo nhiều PathData khác nhau và random giữa chúng

## Script References
- `PathData.cs` - ScriptableObject lưu thông tin path
- `ObstaclePathMovement.cs` - Component điều khiển obstacle di chuyển theo path
- `PathDataEditor.cs` - Custom editor để vẽ path
- `PathVisualizer.cs` - Visualize path trong Scene

## Troubleshooting

### Obstacle không di chuyển theo path
✓ Kiểm tra Obstacle prefab có component **ObstaclePathMovement** chưa
✓ Kiểm tra LevelData có set Movement Type = Path chưa
✓ Kiểm tra PathData có ít nhất 2 điểm chưa

### Path không hiển thị khi vẽ
✓ Đảm bảo đã click "Start Drawing Path"
✓ Chuyển sang Scene view (không phải Game view)
✓ Zoom vào vị trí phù hợp trong Scene

### Obstacle spawn nhưng không di chuyển
✓ Kiểm tra Path Speed > 0
✓ Kiểm tra PathData đã được gán vào LevelData
