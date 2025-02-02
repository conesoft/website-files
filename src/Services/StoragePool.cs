using Conesoft.Hosting;
using System.Management;

[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416")]
record StoragePool(string FriendlyName, ulong Size, ulong AllocatedSize)
{
    private static ManagementObject? Query =>
        Safe.Try(() =>
            new ManagementObjectSearcher(
                scope: "root\\Microsoft\\Windows\\Storage",
                queryString: "SELECT * FROM MSFT_StoragePool WHERE IsPrimordial=False"
            )
            .Get()
            .Cast<ManagementObject>()
            .FirstOrDefault()
        );

    public static StoragePool? Current => Query switch
    {
        ManagementObject pool => new StoragePool(
            FriendlyName: Safe.Try(() => pool.GetPropertyValue(nameof(FriendlyName)) as string) ?? "",
            Size: Safe.Try(() => (ulong)pool.GetPropertyValue(nameof(Size))),
            AllocatedSize: Safe.Try(() => (ulong)pool.GetPropertyValue(nameof(AllocatedSize)))
        ),
        null => null
    };
};