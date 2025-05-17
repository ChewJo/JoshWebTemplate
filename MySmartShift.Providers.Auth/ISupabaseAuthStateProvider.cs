using Supabase.Gotrue;

namespace MySmartShift.Providers.Auth;
public interface ISupabaseAuthStateProvider
{
    public Task Auth_OnAuthStateChanged(Session? session);
}
