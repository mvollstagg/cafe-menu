using IAndOthers.Core.Data.Entity;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Omu.ValueInjecter;

public abstract class AdminController<TEntity, TCreateModel, TUpdateModel, TViewModel, TContext> : Controller
    where TEntity : class, IIOEntity, new()
    where TCreateModel : class, new()
    where TUpdateModel : class, new()
    where TViewModel : class, new()
    where TContext : DbContext, new()
{
    public readonly IIORepository<TEntity, TContext> _repository;

    protected AdminController(IIORepository<TEntity, TContext> repository)
    {
        _repository = repository;
    }

    public virtual async Task<IActionResult> Index()
    {
        ViewData["Title"] = typeof(TEntity).Name;
        var result = await _repository.GetListAsync();
        if (result.Meta.Status != IOResultStatusEnum.Success) return View("Error");

        var viewModels = result.Data.Select(entity => (TViewModel)new TViewModel().InjectFrom(entity)).ToList();
        return View("Index", viewModels);
    }

    public virtual async Task<IActionResult> Create()
    {
        await SetListViewData(null);
        ViewData["Title"] = typeof(TEntity).Name;
        return View("Create", new TCreateModel());
    }

    [HttpPost]
    public virtual async Task<IActionResult> Create(TCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            await SetListViewData(null);
            ViewData["Title"] = typeof(TEntity).Name;
            return View("Create", model);
        }

        var entity = (TEntity)new TEntity().InjectFrom(model);
        var result = await _repository.InsertAsync(entity);

        if (result.Status != IOResultStatusEnum.Success) return View("Error");
        return RedirectToAction("Index");
    }

    public virtual async Task<IActionResult> Edit(long id)
    {
        var result = await _repository.GetAsync(x => x.Id == id);
        if (result.Meta.Status != IOResultStatusEnum.Success || result.Data == null) return NotFound();

        var updateModel = (TUpdateModel)new TUpdateModel().InjectFrom(result.Data);

        await SetListViewData(updateModel);
        ViewData["Title"] = typeof(TEntity).Name;

        return View("Edit", updateModel);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Edit(TUpdateModel model)
    {
        if (!ModelState.IsValid)
        {
            await SetListViewData(model);
            ViewData["Title"] = typeof(TEntity).Name;
            return View("Edit", model);
        }

        var entity = (TEntity)new TEntity().InjectFrom(model);
        var result = await _repository.UpdateAsync(entity);

        if (result.Status != IOResultStatusEnum.Success) return View("Error");
        return RedirectToAction("Index");
    }

    public virtual async Task<IActionResult> Delete(long id)
    {
        var data = await _repository.GetAsync(x => x.Id == id);
        if (data.Meta.Status != IOResultStatusEnum.Success || data.Data == null) return NotFound();
        var result = await _repository.DeleteAsync(id);
        if (result.Status != IOResultStatusEnum.Success) return View("Error");
        return RedirectToAction("Index");
    }

    public virtual async Task SetListViewData(object? model)
    {
    }
}
