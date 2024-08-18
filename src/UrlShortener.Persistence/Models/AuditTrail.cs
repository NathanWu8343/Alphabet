using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Persistence.Models
{
    /// <summary>
    /// TrailType 枚舉，表示審計類型
    /// </summary>
    public enum TrailType : byte
    {
        None = 0,    // 無操作
        Create = 1,  // 新增操作
        Update = 2,  // 更新操作
        Delete = 3   // 刪除操作
    }

    /// <summary>
    /// 審計條目類
    /// </summary>
    public class AuditTrail
    {
        public Guid Id { get; set; } // 審計條目ID
        public Guid? UserId { get; set; } // 用戶ID
        public TrailType TrailType { get; set; } // 審計類型
        public DateTime DateAtUtc { get; set; } // 審計時間（UTC）
        public string EntityName { get; set; } // 實體名稱
        public string? PrimaryKey { get; set; } // 主鍵值
        public Dictionary<string, object?> OldValues { get; set; } = new(); // 舊值
        public Dictionary<string, object?> NewValues { get; set; } = new(); // 新值
        public List<string> ChangedColumns { get; set; } = new(); // 變更的列
    }
}