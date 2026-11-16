namespace AutoTrade.Core.Constants;

public static class Permissions
{
    public static class VehicleBrands
    {
        public const string Read = "Permissions.VehicleBrands.Read";
        public const string Create = "Permissions.VehicleBrands.Create";
        public const string Update = "Permissions.VehicleBrands.Update";
        public const string Delete = "Permissions.VehicleBrands.Delete";
    }

    public static class Roles
    {
        public const string Read = "Permissions.Roles.Read";
        public const string Create = "Permissions.Roles.Create";
        public const string Update = "Permissions.Roles.Update";
        public const string Delete = "Permissions.Roles.Delete";
        public const string Assign = "Permissions.Roles.Assign";
    }

    public static List<string> GetAllPermissions()
    {
        return new List<string>
        {
            VehicleBrands.Read,
            VehicleBrands.Create,
            VehicleBrands.Update,
            VehicleBrands.Delete,
            Roles.Read,
            Roles.Create,
            Roles.Update,
            Roles.Delete,
            Roles.Assign
        };
    }
}