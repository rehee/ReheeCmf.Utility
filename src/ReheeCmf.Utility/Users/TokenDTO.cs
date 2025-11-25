using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ReheeCmf.Users
{
	public class TokenDTO
	{
		public string? TokenString { get; set; }

		public string? RefreshTokenString { get; set; }

		public DateTime? ExpireUCTTime { get; set; }

		public string? UserId { get; set; }

		public string? UserName { get; set; }

		public string? UserEmail { get; set; }

		public string? Avatar { get; set; }

		public IEnumerable<string>? Permissions { get; set; }

		public IEnumerable<string>? Roles { get; set; }

		public bool? IsSystemToken { get; set; }

		public ulong ExpireSecond { get; set; }

		public Guid? TenantID { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public bool? Impersonate { get; set; }

		public Dictionary<string, string>? Claims { get; set; }
	}
}
