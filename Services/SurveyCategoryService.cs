using Ankets.Data;
using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using Ankets.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ankets.Services
{
    public class SurveyCategoryService : ISurveyCategoryService
    {
        private readonly AppDbContext _context;

        public SurveyCategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SurveyCategoryResponseDto>> GetAllCategoriesAsync()
        {
            return await _context.SurveyCategories
                .Select(c => new SurveyCategoryResponseDto
                {
                    CategoriesId = c.CategoriesId,
                    CategoriesName = c.CategoriesName,
                    CategoriesDescription = c.CategoriesDescription,
                    Color = c.Color
                }).ToListAsync();
        }

        public async Task<SurveyCategoryResponseDto> GetCategoryByIdAsync(int id)
        {
            var category = await _context.SurveyCategories.FindAsync(id);
            if (category == null) return null;

            return new SurveyCategoryResponseDto
            {
                CategoriesId = category.CategoriesId,
                CategoriesName = category.CategoriesName,
                CategoriesDescription = category.CategoriesDescription,
                Color = category.Color
            };
        }

        public async Task<SurveyCategoryResponseDto> CreateCategoryAsync(SurveyCategoryRequestDto requestDto)
        {
            var category = new SurveyCategory
            {
                CategoriesName = requestDto.CategoriesName,
                CategoriesDescription = requestDto.CategoriesDescription,
                Color = requestDto.Color
            };
            _context.SurveyCategories.Add(category);
            await _context.SaveChangesAsync();

            return new SurveyCategoryResponseDto
            {
                CategoriesId = category.CategoriesId,
                CategoriesName = category.CategoriesName,
                CategoriesDescription = category.CategoriesDescription,
                Color = category.Color
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, SurveyCategoryRequestDto requestDto)
        {
            var category = await _context.SurveyCategories.FindAsync(id);
            if (category == null) return false;

            category.CategoriesName = requestDto.CategoriesName;
            category.CategoriesDescription = requestDto.CategoriesDescription;
            category.Color = requestDto.Color;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.SurveyCategories.FindAsync(id);
            if (category == null) return false;

            _context.SurveyCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SurveyCategoryResponseDto>> ImportCategoriesAsync(List<SurveyCategoryRequestDto> categories)
        {
            if (categories == null || !categories.Any()) return new List<SurveyCategoryResponseDto>();

            var entities = categories.Select(dto => new SurveyCategory
            {
                CategoriesName = dto.CategoriesName,
                CategoriesDescription = dto.CategoriesDescription,
                Color = dto.Color
            }).ToList();

            _context.SurveyCategories.AddRange(entities);
            await _context.SaveChangesAsync();

            return entities.Select(c => new SurveyCategoryResponseDto
            {
                CategoriesId = c.CategoriesId,
                CategoriesName = c.CategoriesName,
                CategoriesDescription = c.CategoriesDescription,
                Color = c.Color
            }).ToList();
        }
    }
}
