﻿using eMine.Lib.Entities;
using eMine.Lib.Entities.Fleet;
using eMine.Models.Fleet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMine.Lib.Repositories.Fleet
{
    public class SparePartRepository : BaseRepository
    {
        public SparePartRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //SparePartManufacturer
        public async Task<List<ListItem<int, string>>> SparePartManufacturerListItemGet()
        {
            var query = from spm in dbContext.SparePartManufacturers
                        where spm.DeletedInd == false
                        && spm.CompanyId == profile.CompanyId
                        orderby spm.SparePartManufacturerId ascending
                        select new ListItem<int, string>()
                        {
                            Key = spm.SparePartManufacturerId,
                            Item = spm.VehicleModelId.ToString()
                        };

            return await query.ToListAsync();
        }

        public async Task <SparePartManufacturerModel> SparePartManufacturerGet(int SparePartManufacturerId)
        {

            var query = from spm in dbContext.SparePartManufacturers
                        where spm.SparePartManufacturerId == SparePartManufacturerId
                        && spm.DeletedInd == false
                        && spm.CompanyId == profile.CompanyId
                        select new SparePartManufacturerModel
                        {
                            SparePartManufacturerId = spm.SparePartManufacturerId,
                            VehicleManufacturerId = spm.VehicleManufacturerId,
                            VehicleModelId = spm.VehicleModelId
                        };


            SparePartManufacturerModel model = await query.FirstOrDefaultAsync();

            return model;
        }

        public async Task SparePartManufacturerAdd(SparePartManufacturerModel model)
        {
            SparePartManufacturerEntity entity = new SparePartManufacturerEntity()
            {
                SparePartManufacturerId = model.SparePartManufacturerId,
                SparePartId = model.SparePartId,
                VehicleManufacturerId = model.VehicleManufacturerId,
                VehicleModelId = model.VehicleModelId,
                VehicleTypeId = model.VehicleTypeId
            };
            dbContext.SparePartManufacturers.Add(entity);
            await  dbContext.SaveChangesAsync();

        }

        public async Task SparePartManufacturerUpdate(SparePartManufacturerModel model)
        {
            //Update the SparePartManufacturerModel Entity first
            SparePartManufacturerEntity entity = (from spm in dbContext.SparePartManufacturers where spm.SparePartManufacturerId == model.SparePartManufacturerId && spm.CompanyId == profile.CompanyId select spm).First();
            entity.SparePartManufacturerId = model.SparePartManufacturerId;
            entity.SparePartId = model.SparePartId;
            entity.VehicleManufacturerId = model.VehicleManufacturerId;
            entity.VehicleModelId = model.VehicleModelId;
            entity.VehicleTypeId = model.VehicleTypeId;
            entity.UpdateAuditFields();
            dbContext.SparePartManufacturers.Update(entity);
            await dbContext.SaveChangesAsync();

        }

        public async Task  SparePartManufacturerModelSave(SparePartManufacturerModel model)
        {
            if (model.SparePartManufacturerId == 0)
            {
                 await SparePartManufacturerAdd(model);
            }
            else
            {
               await SparePartManufacturerUpdate(model);
            }
        }

        public async Task <SparePartOrderModel> SparePartOrderGet(int sparePartOrderId)
        {
            SparePartOrderModel model = null;
            if (sparePartOrderId == 0)
            {
                model = new SparePartOrderModel();
                model.SparePartOrderId = sparePartOrderId;
                model.SparePartId = 0;
                model.OrderedUnits = 0;
                model.UnitCost = 0;
                model.OrderedUTCdatetime = null;
                model.DeliveredUTCdatetime = null;
                model.FollowupEmailAddress = "";
                model.FollowupPhoneNum = "";
            }
            else
            {
                var query = from order in dbContext.SparePartOrders
                            where order.SparePartOrderId == sparePartOrderId
                            && order.DeletedInd == false
                            && order.CompanyId == profile.CompanyId
                            select new SparePartOrderModel
                            {
                                SparePartOrderId = order.SparePartOrderId,
                                SparePartId = order.SparePartId,
                                OrderedUnits = order.OrderedUnits,
                                UnitCost = order.UnitCost,
                                OrderedUTCdatetime = order.OrderedUTCdatetime,
                                DeliveredUTCdatetime = order.DeliveredUTCdatetime,
                                FollowupEmailAddress = order.FollowupEmailAddress,
                                FollowupPhoneNum = order.FollowupPhoneNum
                            };

                model = await query.FirstOrDefaultAsync();
            }

            return model;
        }

        public async Task SparePartOrderAdd(SparePartOrderModel model)
        {
            SparePartOrderEntity entity = new SparePartOrderEntity()
            {
                SparePartId = model.SparePartId,
                OrderedUnits = model.OrderedUnits,
                UnitCost = model.UnitCost,
                OrderedUTCdatetime = model.OrderedUTCdatetime,
                DeliveredUTCdatetime = model.DeliveredUTCdatetime,
                FollowupEmailAddress = model.FollowupEmailAddress,
                FollowupPhoneNum = model.FollowupPhoneNum
            };
            dbContext.SparePartOrders.Add(entity);

            //Get the newly added 

            SparePartEntity result = (from part in dbContext.SpareParts
                                      where part.SparePartId == model.SparePartId
                                      && part.DeletedInd == false
                                      && part.CompanyId == profile.CompanyId
                                      select part).SingleOrDefault();

            result.AvailableQuantity += model.OrderedUnits;

           await dbContext.SaveChangesAsync();

        }

        public async Task SparePartOrderUpdate(SparePartOrderModel model)
        {
            //Update the VehicleService Entity first
            SparePartOrderEntity entity = (from order in dbContext.SparePartOrders where order.SparePartOrderId == model.SparePartOrderId && order.CompanyId == profile.CompanyId select order).First();
            int diff = entity.OrderedUnits - model.OrderedUnits;
            entity.SparePartId = model.SparePartId;
            entity.OrderedUnits = model.OrderedUnits;
            entity.UnitCost = model.UnitCost;
            entity.OrderedUTCdatetime = model.OrderedUTCdatetime;
            entity.DeliveredUTCdatetime = model.DeliveredUTCdatetime;
            entity.FollowupEmailAddress = model.FollowupEmailAddress;
            entity.FollowupPhoneNum = model.FollowupPhoneNum;
            entity.UpdateAuditFields();
            dbContext.SparePartOrders.Update(entity);
            dbContext.SaveChanges();

            //Get the newly added 
            SparePartEntity result = (from part in dbContext.SpareParts
                                      where part.SparePartId == model.SparePartId
                                      && part.DeletedInd == false
                                      && part.CompanyId == profile.CompanyId
                                      select part).SingleOrDefault();

            result.AvailableQuantity -= diff;

            await  dbContext.SaveChangesAsync();

        }

        public async Task  SparePartOrderSave(SparePartOrderModel model)
        {
            if (model.SparePartOrderId == 0)
            {
                 await SparePartOrderAdd(model);
            }
            else
            {
               await SparePartOrderUpdate(model);
            }
        }

        public async Task SparePartUpdate(SparePartModel model)
        {
            SparePartEntity entity = (from spart in dbContext.SpareParts where spart.SparePartId == model.SparePartId && spart.CompanyId == profile.CompanyId select spart).First();

            entity.SparePartId = model.SparePartId;
            entity.AvailableQuantity = model.Quantity;
            entity.SparePartName = model.Name;
            entity.ManufacturingBrand = model.ManufacturingBrand;
            entity.SparePartDescription = model.Description;
            entity.UpdateAuditFields();

            SparePartManufacturerEntity spamen = (from spam in dbContext.SparePartManufacturers where spam.SparePartId == model.SparePartId && spam.CompanyId == profile.CompanyId select spam ).FirstOrDefault();

            if (model.VehicleTypeId != 0 || model.VehicleManufacturerId != 0 || model.VehicleModelId != 0)
            {
                if(spamen == null)
                {
                    SparePartManufacturerEntity spam = new SparePartManufacturerEntity();
                    spam.SparePartId = model.SparePartId;
                    spam.VehicleModelId = model.VehicleModelId;
                    spam.VehicleTypeId = model.VehicleTypeId;
                    spam.VehicleManufacturerId = model.VehicleManufacturerId;
                    dbContext.SparePartManufacturers.Add(spam);
                }
                else
                {
                    spamen.VehicleModelId = model.VehicleModelId;
                    spamen.VehicleTypeId = model.VehicleTypeId;
                    spamen.VehicleManufacturerId = model.VehicleManufacturerId;
                    spamen.DeletedInd = false;
                    dbContext.SparePartManufacturers.Update(spamen);
                }               
            }
            else
            {
                    spamen.DeletedInd = false;
                    dbContext.SparePartManufacturers.Update(spamen);
            }

            dbContext.SpareParts.Update(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task SparePartAdd(SparePartModel model)
        {
            SparePartEntity entity = new SparePartEntity()
            {
                SparePartId = model.SparePartId,
                AvailableQuantity = model.Quantity,
                SparePartName = model.Name,
                ManufacturingBrand = model.ManufacturingBrand,
                SparePartDescription = model.Description
            };


            dbContext.SpareParts.Add(entity);
            dbContext.SaveChanges();
            model.SparePartId = entity.SparePartId;

            if (model.VehicleTypeId != 0 || model.VehicleManufacturerId != 0 || model.VehicleModelId != 0)
            {
                SparePartManufacturerEntity spam = new SparePartManufacturerEntity();
                spam.SparePartId = model.SparePartId;
                spam.VehicleModelId = model.VehicleModelId;
                spam.VehicleTypeId = model.VehicleTypeId;
                spam.VehicleManufacturerId = model.VehicleManufacturerId;
                dbContext.SparePartManufacturers.Add(spam);
                await dbContext.SaveChangesAsync();
            }          

        }

        public async Task  SparePartSave(SparePartModel model)
        {
            if (model.SparePartId == 0)
            {
               await SparePartAdd(model);
            }
            else
            {
                await SparePartUpdate(model);
            }
        }

        public async Task <SparePartModel> SparePartGet(int SparePartId, VehicleRepository vehicleRepository)
        {
            SparePartModel model = null;
            if (SparePartId == 0)
            {
                model = new SparePartModel();
                model.SparePartId = 0;
                model.Quantity = 0;
                model.Name = "";
                model.ManufacturingBrand = "";
                model.Description = "";
                model.VehicleModelId = 0;
                model.VehicleTypeId = 0;
                model.VehicleManufacturerId = 0;
            }
            else
            {
                var sparePartGetQuery = from sPart in dbContext.SpareParts
                                        where sPart.SparePartId == SparePartId
                                        && sPart.DeletedInd == false
                                        && sPart.CompanyId == profile.CompanyId
                                        select new SparePartModel
                                        {
                                            SparePartId = sPart.SparePartId,
                                            Quantity = sPart.AvailableQuantity,
                                            Name = sPart.SparePartName,
                                            ManufacturingBrand = sPart.ManufacturingBrand,
                                            Description = sPart.SparePartDescription
                                        };

                model = await sparePartGetQuery.FirstOrDefaultAsync();

                SparePartManufacturerEntity spamen = (from spam in dbContext.SparePartManufacturers where spam.SparePartId == model.SparePartId && spam.CompanyId == profile.CompanyId select spam).FirstOrDefault();

                if (spamen != null)
                {
                    model.VehicleModelId = spamen.VehicleModelId;
                    model.VehicleTypeId = spamen.VehicleTypeId;
                    model.VehicleManufacturerId = spamen.VehicleManufacturerId;
                }
                else
                {
                    model.VehicleModelId = 0;
                    model.VehicleTypeId = 0;
                    model.VehicleManufacturerId = 0;
                }

            }

            model.VehicleTypeList = await vehicleRepository.VehicleTypeListItemGet();
            model.ManufacturerList =await vehicleRepository.VehicleManufacturerListItemGet();
            model.VehicleModelList =await vehicleRepository.VehicleManufactureModelGet();

            return model;
        }

        public async Task <SparePartDetailsModel> SparePartDetailsGet(int sparePartId, VehicleRepository vehicleRepository)
        {
            var sparePartGetQuery = from sPart in dbContext.SpareParts
                                    where sPart.SparePartId == sparePartId
                                    && sPart.DeletedInd == false
                                    && sPart.CompanyId == profile.CompanyId
                                    select new SparePartDetailsModel
                                    {
                                        SparePartId = sPart.SparePartId,
                                        Quantity = sPart.AvailableQuantity,
                                        Name = sPart.SparePartName,
                                        ManufacturingBrand = sPart.ManufacturingBrand,
                                        Description = sPart.SparePartDescription
                                    };

            SparePartDetailsModel model = await sparePartGetQuery.FirstOrDefaultAsync();

            if (model == null) return model;

            SparePartManufacturerEntity spamen = null;

            if (sparePartId != 0)
            {
                spamen = (from spam in dbContext.SparePartManufacturers where spam.SparePartId == model.SparePartId && spam.CompanyId == profile.CompanyId select spam).FirstOrDefault();
            }

            if (spamen != null)
            {
                var modelQuery = (from models in dbContext.VehicleModels
                                  where models.VehicleModelId == spamen.VehicleModelId
                                  && models.DeletedInd == false
                                  && models.CompanyId == profile.CompanyId
                                  select models.Description).SingleOrDefault();

                model.VehicleModel = modelQuery.ToString();

                var typeQuery = (from types in dbContext.VehicleTypes
                                  where types.VehicleTypeId == spamen.VehicleTypeId
                                  && types.DeletedInd == false
                                  && types.CompanyId == profile.CompanyId
                                 select types.VehicleTypeName).SingleOrDefault();

                model.VehicleType = typeQuery.ToString();


                var manfQuery = (from manufacturers in dbContext.VehicleManufacturers
                                 where manufacturers.VehicleManufacturerId == spamen.VehicleManufacturerId
                                 && manufacturers.DeletedInd == false
                                 && manufacturers.CompanyId == profile.CompanyId
                                 select manufacturers.Description).SingleOrDefault();

                model.VehicleManufacturer = manfQuery.ToString();

                //model.VehicleManufacturer = spamen.VehicleManufacturerId;
            }
            else
            {
                model.VehicleModel = "";
                model.VehicleType = "";
                model.VehicleManufacturer = "";
            }


            var ordersQuery = from order in dbContext.SparePartOrders
                              where order.SparePartId == sparePartId
                              && order.DeletedInd == false
                              && order.CompanyId == profile.CompanyId
                              select new SparePartOrderModel
                              {
                                  SparePartOrderId = order.SparePartOrderId,
                                  SparePartId = order.SparePartId,
                                  OrderedUnits = order.OrderedUnits,
                                  UnitCost = order.UnitCost,
                                  OrderedUTCdatetime = order.OrderedUTCdatetime,
                                  DeliveredUTCdatetime = order.DeliveredUTCdatetime,
                                  FollowupEmailAddress = order.FollowupEmailAddress,
                                  FollowupPhoneNum = order.FollowupPhoneNum,
                                  ConsumedUnits = order.ConsumedUnits 
                              };

            model.Orders = await ordersQuery.ToListAsync();

            return model;

        }


        public async Task <List<SparePartModel>> SparePartListGet()
        {
            var query = from parts in dbContext.SpareParts
                        where parts.DeletedInd == false
                        && parts.CompanyId == profile.CompanyId
                        orderby parts.SparePartName ascending
                        select new SparePartModel
                        {
                            SparePartId = parts.SparePartId,
                            Quantity = parts.AvailableQuantity,
                            Name = parts.SparePartName,
                            ManufacturingBrand = parts.ManufacturingBrand,
                            Description = parts.SparePartDescription
                          
                        };

            return  await query.ToListAsync();
        }


        public async Task <List<ListItem<int, string>>> SparePartListItemGet()
        {
            var query = from parts in dbContext.SpareParts
                        where parts.DeletedInd == false
                        && parts.CompanyId == profile.CompanyId
                        orderby parts.SparePartName ascending
                        select new ListItem<int, string>()
                        {
                            Key = parts.SparePartId,
                            Item = parts.SparePartName
                        };

            return await query.ToListAsync();
        }


        //RamPras algorithm to get the total cost
        public async Task<decimal> GetSparePartsCost(SparePartModel spvm, VehicleServiceViewModel vsvm, int totalparts, bool bUpdate = false)
        {
            decimal totalcost = 0;

            int totalAvailable = 0;
                        
            var partorderquery = (from order in dbContext.SparePartOrders
                                  where order.SparePartId == spvm.SparePartId && order.OrderedUnits > order.ConsumedUnits
                                  && order.DeletedInd == false
                                  && order.CompanyId == profile.CompanyId
                                  orderby order.DeliveredUTCdatetime ascending
                                  select order).ToList();

            foreach (SparePartOrderEntity order in partorderquery) { totalAvailable += order.OrderedUnits - order.ConsumedUnits; }

            // Do not proceed with Save if there are no parts.
            if (totalAvailable < totalparts) return totalAvailable - totalparts;
            
            //Get the price and update the consumed units 
            int totalNeeded = totalparts;
            foreach (SparePartOrderEntity order in partorderquery)
            {
                if (totalNeeded < (order.OrderedUnits - order.ConsumedUnits) )
                {
                    totalcost = totalcost + (order.UnitCost * totalNeeded);
                    if (bUpdate)
                    {
                        order.ConsumedUnits += totalNeeded;
                        dbContext.SparePartOrders.Update(order);
                        await CreateVehicleServiceSparePartOrderLink(spvm, vsvm, order, order.OrderedUnits - order.ConsumedUnits);
                    }
                    break;
                }

                totalNeeded -= (order.OrderedUnits - order.ConsumedUnits);
                totalcost = totalcost + (order.UnitCost * (order.OrderedUnits - order.ConsumedUnits));
                if (bUpdate)
                {
                    await CreateVehicleServiceSparePartOrderLink(spvm, vsvm, order, order.OrderedUnits - order.ConsumedUnits);
                    order.ConsumedUnits = order.OrderedUnits;
                    dbContext.SparePartOrders.Update(order);
                }
            }

            return totalcost;
        }

        //Create the VehicleService and SparePartOrders link table
        public async Task CreateVehicleServiceSparePartOrderLink(SparePartModel spvm, VehicleServiceViewModel model, SparePartOrderEntity order, int nUnitsToconsume)
        {
            VehicleServiceSparePartOrderEntity servicePartOrder = null;
            servicePartOrder = 
                (from vspentity in dbContext.VehicleServiceSparePartOrders
                 where vspentity.DeletedInd == false && vspentity.SparePartOrderId == order.SparePartOrderId
                 && vspentity.VehicleServiceId == model.VehicleServiceId
                 && vspentity.CompanyId == profile.CompanyId
                 select vspentity).SingleOrDefault();

            if (servicePartOrder == null)
            {
                servicePartOrder = new VehicleServiceSparePartOrderEntity()
                {
                    SparePartOrderId = order.SparePartOrderId,
                    ConsumedUnits = nUnitsToconsume,
                    VehicleServiceId = model.VehicleServiceId
                };
                dbContext.VehicleServiceSparePartOrders.Add(servicePartOrder);
            }
            else
            {
                servicePartOrder.ConsumedUnits += nUnitsToconsume;
            }
            await dbContext.SaveChangesAsync();
        }
    
        
    }
}