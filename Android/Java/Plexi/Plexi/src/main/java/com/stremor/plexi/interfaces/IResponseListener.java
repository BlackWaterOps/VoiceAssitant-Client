package com.stremor.plexi.interfaces;

/**
 * Created by jeffschifano on 10/29/13.
 */
public interface IResponseListener {
    public void onQueryResponse(Object queryResponse);

    public void onInternalError();
}
