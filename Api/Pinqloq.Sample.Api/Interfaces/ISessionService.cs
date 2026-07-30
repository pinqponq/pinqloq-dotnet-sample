using Pinqloq.Sample.Api.Models;

namespace Pinqloq.Sample.Api.Interfaces;

public interface ISessionService
{
    SessionModel.GetAll.ReturnData GetAll(SessionModel.GetAll.Request request);

    SessionModel.Create.ReturnData Create(SessionModel.Create.Request request);

    SessionModel.Update.ReturnData Update(SessionModel.Update.Request request);

    BaseResponseModel Delete(SessionModel.Delete.Request request);
}
