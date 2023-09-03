using System.Management;

record StoragePool(string FriendlyName, ulong Size, ulong AllocatedSize)
{
    private static IEnumerable<ManagementObject> Query =>
        new ManagementObjectSearcher(
            scope: "root\\Microsoft\\Windows\\Storage",
            queryString: "SELECT * FROM MSFT_StoragePool WHERE IsPrimordial=False"
        )
        .Get()
        .Cast<ManagementObject>();

    public static StoragePool? Current => Query.FirstOrDefault() switch
    {
        ManagementObject pool => new StoragePool(
            FriendlyName:  (string) pool.GetPropertyValue(nameof(FriendlyName)),
            Size:          (ulong)  pool.GetPropertyValue(nameof(Size)),
            AllocatedSize: (ulong)  pool.GetPropertyValue(nameof(AllocatedSize))
        ),
        null => null
    };
};