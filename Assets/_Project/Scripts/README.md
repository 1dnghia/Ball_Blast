# Ball Blast - Code Structure

## 📁 Folder Organization

```
Assets/_Project/Scripts/
├── Entities/              # Game entities (modular components)
│   ├── Obstacle/
│   │   ├── Obstacle.cs              # Main coordinator (gọn gàng)
│   │   ├── ObstacleHealth.cs        # HP, damage, UI
│   │   ├── ObstacleMovement.cs      # Di chuyển, physics, bounce
│   │   ├── ObstacleSplitter.cs      # Logic split
│   │   └── ObstacleCoinDropper.cs   # Spawn coin
│   │
│   ├── Weapon/
│   │   ├── WeaponRoot.cs            # Main coordinator (gọn gàng)
│   │   ├── WeaponMovement.cs        # Di chuyển weapon, wheels
│   │   ├── WeaponShooter.cs         # Bắn đạn
│   │   ├── WeaponRecoil.cs          # Hiệu ứng giật
│   │   └── WeaponCoinCollector.cs   # Hút coin
│   │
│   ├── Bullet/
│   │   └── Bullet.cs                # Bullet logic
│   │
│   └── Coin/
│       └── Coin.cs                  # Coin logic
│
├── Managers/              # Game managers
│   ├── LevelManager.cs
│   ├── ObstacleSpawner.cs
│   ├── ScoreManager.cs
│   └── ObjectPool.cs
│
├── UI/                    # UI components
│   └── UIManager.cs
│
├── Events/                # Event system
│   ├── EventBus.cs
│   ├── IGameEvent.cs
│   └── GameEvents.cs
│
├── Data/                  # ScriptableObjects
│   ├── ObstacleData.cs
│   ├── LevelData.cs
│   └── BulletData.cs
│
└── Utils/                 # Utilities
    └── ScreenBounds.cs
```

## 🎯 Design Principles

### Single Responsibility
- Mỗi class làm **1 việc duy nhất**
- ObstacleHealth chỉ quản lý HP/damage
- ObstacleMovement chỉ quản lý di chuyển
- Dễ đọc, dễ sửa, dễ test

### Component-Based Architecture
- **Obstacle** = Coordinator gọn (100 lines)
  - Kết hợp 4 components: Health, Movement, Splitter, CoinDropper
  
- **WeaponRoot** = Coordinator gọn (10 lines!)
  - Kết hợp 4 components: Movement, Shooter, Recoil, CoinCollector

### Modular & Reusable
- Mỗi component độc lập
- Có thể tái sử dụng cho entities khác
- Easy to add/remove features

## 🔧 How to Use

### Setup Obstacle Prefab
```
GameObject (Obstacle)
├── Obstacle.cs          ← Main coordinator
├── ObstacleHealth.cs
├── ObstacleMovement.cs
├── ObstacleSplitter.cs
├── ObstacleCoinDropper.cs
├── Rigidbody2D
└── Collider2D
```

### Setup Weapon Prefab
```
GameObject (WeaponRoot)
├── WeaponRoot.cs           ← Main coordinator
├── WeaponMovement.cs
├── WeaponShooter.cs
├── WeaponRecoil.cs
└── WeaponCoinCollector.cs
```

## ✅ Benefits

1. **Dễ đọc**: Mỗi file < 150 lines, không cần scroll nhiều
2. **Dễ sửa**: Sửa Health? Vào ObstacleHealth.cs thôi
3. **Dễ mở rộng**: Thêm feature? Tạo component mới
4. **Không sợ conflict**: Mỗi người làm 1 component riêng
5. **Easy testing**: Test từng component độc lập

## 📝 Code Style

- **Clear naming**: ObstacleHealth, not OH
- **Region grouping**: #region Initialization
- **Comments**: Chỉ khi cần thiết
- **SerializeField**: Expose settings trong Inspector
- **Private by default**: Public chỉ khi cần

---
**Refactored**: December 2025  
**Pattern**: Component-Based Architecture  
**Goal**: Clean, Readable, Maintainable Code
