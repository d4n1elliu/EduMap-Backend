using EduMap.Data;

namespace EduMap.Services;

public class BuddySystenServices
{
    private readonly AppDbContext _context;

    public BuddySystenServices(AppDbContext context)
    {
        _context = context;
    }
}