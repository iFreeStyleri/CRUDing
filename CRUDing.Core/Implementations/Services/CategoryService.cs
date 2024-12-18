using AutoMapper;
using CRUDing.Core.Abstractions.Repositories;
using CRUDing.Core.Abstractions.Services;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.DTOs.Category;
using CRUDing.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CRUDing.Core.Implementations.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IValidator<AddCategoryDTO> _addCategoryValidator;
        private readonly IValidator<UpdateCategoryDTO> _updateCategoryValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IBaseRepository<Category> categoryRepository,
            IValidator<AddCategoryDTO> addCategoryValidator,
            IValidator<UpdateCategoryDTO> updateCategoryValidator,
            IMapper mapper,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _addCategoryValidator = addCategoryValidator;
            _updateCategoryValidator = updateCategoryValidator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IBaseResponse<Category>> GetCategory(int id)
        {
            try
            {
                _logger.LogInformation("Поиск категории {id}", id);
                var result = await _categoryRepository.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning(
                        "Категория не найдена: {id}", id);
                    return new BaseResponse<Category>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Category not found"
                    };
                }
                _logger.LogInformation("Найдена категория {@category}", result);
                return new BaseResponse<Category>
                {
                    Code = HttpStatusCode.OK,
                    Data = result,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<Category>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<object>> AddCategory(AddCategoryDTO dto)
        {
            try
            {
                var validResult = await _addCategoryValidator.ValidateAsync(dto);
                if (!validResult.IsValid)
                {
                    _logger.LogWarning(
                        "Неправильный запрос: {@dto}\n {errors}", dto, validResult.Errors.Select(s => s.ErrorMessage));
                    return new BaseResponse<object>
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = string.Join('\n', validResult.Errors.Select(s => s.ErrorMessage))
                    };
                }
                var category = _mapper.Map<Category>(dto);
                await _categoryRepository.AddAsync(category);
                _logger.LogInformation("Категория добавлена {@category}", category);
                return new BaseResponse<object> {Code = HttpStatusCode.OK};
            }
            catch (Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<object>
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
            }

        }

        public async Task<IBaseResponse<List<Category>>> GetOffsetCategory(int page)
        {
            try
            {
                _logger.LogInformation("Получение категории на странице {page}...", page);
                var result = await _categoryRepository.GetAll()
                    .Where(w => !w.IsDeleted).OrderBy(o => o.Id).Skip(page * 25).Take(25).ToListAsync();
                if (result.Count == 0)
                {
                    _logger.LogInformation("Категории не найдены");
                    return new BaseResponse<List<Category>>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Categories not found"
                    };
                }
                _logger.LogInformation("Найдено {count} на странице {page}...", result.Count, page);
                return new BaseResponse<List<Category>>
                {
                    Code = HttpStatusCode.OK,
                    Data = result,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<List<Category>>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }

        }

        public async Task<IBaseResponse<Category>> RemoveCategory(int id)
        {
            try
            {
                _logger.LogInformation("Удаление категории {id}...", id);
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning(
                        "Категория не найдена: {id}", id);
                    return new BaseResponse<Category>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Category not found"
                    };
                }
                category.IsDeleted = true;
                await _categoryRepository.UpdateAsync(category);
                _logger.LogInformation("Категория {id} удалена", id);
                return new BaseResponse<Category>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<Category>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }

        }

        public async Task<IBaseResponse<Category>> UpdateCategory(UpdateCategoryDTO dto)
        {
            try
            {
                var validResult = await _updateCategoryValidator.ValidateAsync(dto);
                if (!validResult.IsValid)
                {
                    _logger.LogWarning(
                        "Неправильный запрос: {@dto}\n {@errors}", dto, validResult.Errors.Select(s => s.ErrorMessage));
                    return new BaseResponse<Category>
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = string.Join('\n', validResult.Errors.Select(s => s.ErrorMessage))
                    };

                }
                var category = _mapper.Map<Category>(dto);
                var updateCategory = await _categoryRepository.GetByIdAsync(category.Id);
                if (updateCategory == null)
                {
                    _logger.LogWarning(
                        "Категория не найдена: {id}", category.Id);
                    return new BaseResponse<Category>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Category not found"
                    };
                }
                updateCategory.Name = category.Name;
                await _categoryRepository.UpdateAsync(updateCategory);
                _logger.LogInformation(
                    "Категория обновлена: {@category}", updateCategory);
                return new BaseResponse<Category>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<Category>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
