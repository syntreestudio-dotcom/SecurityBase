using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Infrastructure.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ApiResponse<int>> CreateMenuAsync(Menu menu)
    {
        try
        {
            var menuId = await _menuRepository.CreateMenuAsync(menu);
            return new ApiResponse<int> { Success = true, Data = menuId, Message = "Menu created successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<Menu>>> GetMenusAsync()
    {
        try
        {
            var menus = await _menuRepository.GetMenusAsync();
            return new ApiResponse<IEnumerable<Menu>> { Success = true, Data = menus };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<Menu>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> UpdateMenuAsync(Menu menu)
    {
        try
        {
            await _menuRepository.UpdateMenuAsync(menu);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Menu updated successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> DeleteMenuAsync(int menuId)
    {
        try
        {
            await _menuRepository.DeleteMenuAsync(menuId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Menu deleted successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }
}
