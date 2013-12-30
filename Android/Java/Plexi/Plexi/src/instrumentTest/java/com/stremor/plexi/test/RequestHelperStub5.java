package com.stremor.plexi.test;

import com.google.gson.JsonElement;
import com.google.gson.JsonParser;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.Choice;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.ResponderModel;
import com.stremor.plexi.models.ShowModel;
import com.stremor.plexi.models.ShowSimpleModel;
import com.stremor.plexi.util.RequestTask;

/**
 * An IRequestHelper stub implementation which simulates the following scenario:
 *
 * - Fake classification immediately returned. The value of one of the fields is a choice operation.
 * - Auditor returns with 'choice' status.
 * - User provides choice input.
 * - Auditor accepts.
 * - Fake actor.
 *
 * Created by jon on 20.12.2013.
 */
public class RequestHelperStub5 implements IRequestHelper {
    private static JsonParser parser = new JsonParser();

    public static ClassifierModel MODEL_START = new ClassifierModel("stock", "query",
            parser.parse("{\"company\": {\"~choice\": {\"matched_on\": \"Apollo\", \"list\": [{\"text\": \"Apollo Residential Mortgage\", \"data\": {\"stock_exchange\": \"NYSE\", \"symbol\": \"AMTG\", \"name\": \"Apollo Residential Mortgage\"}}, {\"text\": \"Apollo Group\", \"data\": {\"stock_exchange\": \"NASDAQ\", \"symbol\": \"APOL\", \"name\": \"Apollo Group\"}}, {\"text\": \"Apollo Tactical Income Fund Inc.\", \"data\": {\"stock_exchange\": \"NYSE\", \"symbol\": \"AIF\", \"name\": \"Apollo Tactical Income Fund Inc.\"}}, {\"text\": \"Apollo Investment Corporation\", \"data\": {\"stock_exchange\": \"NYSE\", \"symbol\": \"AIY\", \"name\": \"Apollo Investment Corporation\"}}, {\"text\": \"Apollo Global Management\", \"data\": {\"stock_exchange\": \"NYSE\", \"symbol\": \"APO\", \"name\": \"Apollo Global Management\"}}, {\"text\": \"Apollo Commercial Real Estate Finance\", \"data\": {\"stock_exchange\": \"NYSE\", \"symbol\": \"ARI\", \"name\": \"Apollo Commercial Real Estate Finance\"}}, {\"text\": \"Apollo Senior Floating Rate Fund Inc.\", \"data\": {\"stock_exchange\": \"NYSE\", \"symbol\": \"AFT\", \"name\": \"Apollo Senior Floating Rate Fund Inc.\"}}]}}}").getAsJsonObject(),
            new String[] {"share_price"}, null);

    public static Choice[] CHOICES = new Choice[] {
            new Choice("Apollo Residential Mortgage", parser.parse("{\"stock_exchange\": \"NYSE\", \"symbol\": \"AMTG\", \"name\": \"Apollo Residential Mortgage\"}")),
            new Choice("Apollo Group", parser.parse("{\"stock_exchange\": \"NASDAQ\", \"symbol\": \"APOL\", \"name\": \"Apollo Group\"}")),
            new Choice("Apollo Tactical Income Fund Inc.", parser.parse("{\"stock_exchange\": \"NYSE\", \"symbol\": \"AIF\", \"name\": \"Apollo Tactical Income Fund Inc.\"}")),
            new Choice("Apollo Investment Corporation", parser.parse("{\"stock_exchange\": \"NYSE\", \"symbol\": \"AIY\", \"name\": \"Apollo Investment Corporation\"}"))
    };
    public static int CHOICE_IDX = 3;

    public static ResponderModel AUDIT_RESPONSE_1 = new ResponderModel("choice", "payload.company",
            new ShowModel(new ShowSimpleModel("I found multiple results for \"Apollo\". Which one did you mean?", CHOICES), null), "I found multiple results for \"Apollo\". Which one did you mean?");

    public static ResponderModel AUDIT_RESPONSE_2 = new ResponderModel("completed", "stock", null, (ClassifierModel) null);

    public static ClassifierModel MODEL_COMPLETE = new ClassifierModel("stock", "query",
            parser.parse("{\"company\":{\"stock_exchange\":\"NYSE\",\"symbol\":\"AIY\",\"name\":\"Apollo Investment Corporation\"}}").getAsJsonObject(),
            new String[] {"share_price"}, null);

    public static ActorModel ACTOR_RESPONSE = new ActorModel(
            new ShowModel(new ShowSimpleModel("Apollo Investment stock is trading at $21.1, down 0.71%"),
                    parser.parse("{\"item\": {\"opening_price\": 21.27, \"low_price\": 21.010000000000002, \"share_price\": 21.100000000000001, \"stock_exchange\": \"NYSE\", \"trade_volume\": 77515.0, \"52_week_high\": 24.949999999999999, \"average_trade_volume\": 35240.0, \"pe\": 32.490000000000002, \"high_price\": 21.300000000000001, \"share_price_change\": -0.14990000000000001, \"market_cap\": \"4.74 billion\", \"5_day_moving_average\": 21.591200000000001, \"symbol\": \"AIY\", \"Earnings/Share\": 0.65400000000000003, \"share_price_change_percent\": 0.70999999999999996, \"name\": \"Apollo Investment\", \"yield\": 4.6699999999999999, \"52_week_low\": 20.719999999999999, \"share_price_direction\": \"down\"}, \"template\": \"single:stock\"}").getAsJsonObject()),
            "Apollo Investment stock is trading at $21.1, down 0.71%", null);

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              IResponseListener listener) {
        if (type == ClassifierModel.class) {
            ClassifierModel response = null;
            try {
                response = MODEL_START.clone();
            } catch (CloneNotSupportedException e) { /* pass */ }

            listener.onQueryResponse(response);
        }
    }

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              Object data, boolean includeNulls, IResponseListener listener) {
        if (type == ResponderModel.class) {
            // This is an auditor call.
            ClassifierModel pkg = (ClassifierModel) data;

            JsonElement company = pkg.getPayload().get("company");
            ResponderModel responseUncloned = null;

            if (company.isJsonObject() && company.getAsJsonObject().has("~choice")) {
                responseUncloned = AUDIT_RESPONSE_1;
            } else if (company.isJsonObject()) {
                assert company.getAsJsonObject().equals(CHOICES[CHOICE_IDX].getData().getAsJsonObject());
                responseUncloned = AUDIT_RESPONSE_2;
            }

            try {
                listener.onQueryResponse(responseUncloned.clone());
            } catch (CloneNotSupportedException e) { /* pass */ }
        } else if (type == ActorModel.class) {
            // This is an actor call.
            listener.onQueryResponse(ACTOR_RESPONSE.clone());
        }
    }
}
