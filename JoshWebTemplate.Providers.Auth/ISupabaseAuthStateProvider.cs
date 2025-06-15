using Supabase.Gotrue;

namespace JoshWebTemplate.Providers.Auth;
public interface ISupabaseAuthStateProvider
{
    public Task Auth_OnAuthStateChanged(Session? session);
}
