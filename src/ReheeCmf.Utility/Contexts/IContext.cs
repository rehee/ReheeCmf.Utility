using System;
using System.Collections.Generic;
using ReheeCmf.Commons;

namespace ReheeCmf.Contexts
{
	public interface IContext : ISaveChange, IRepository, IWithTenant, ITenantContext, ICrossTenant, IDisposable, ITokenDTOContext
	{
		IServiceProvider? ServiceProvider { get; }

		object? Context { get; }

		object? Query(Type type, bool noTracking, bool readCheck = false);

		object? QueryWithKey(Type type, Type keyType, bool noTracking, object key, bool readCheck = false);

		object? Find(Type type, object key);

		void Add(Type type, object? value);

		void Delete(Type type, object key);

		void TrackEntity(object entity, EnumEntityState enumEntityStatus = EnumEntityState.Modified);

		IEnumerable<KeyValueItemDTO> GetKeyValueItemDTO(Type type);
	}
}
