package com.stremor.plexi.client.views;

import android.content.Context;
import android.util.AttributeSet;
import android.widget.LinearLayout;
import android.widget.ListView;

import com.stremor.plexi.client.ConversationAdapter;
import com.stremor.plexi.client.R;
import com.stremor.plexi.client.models.ConversationItem;

import java.util.ArrayList;
import java.util.List;

public class MainView extends LinearLayout {

    private ListView mConversationView;
    private ConversationAdapter mConversationAdapter;
    private List<ConversationItem> mConversationItems;
    private ViewListener mViewListener;

    public MainView(Context context) {
        super(context);
        setup();
    }

    public MainView(Context context, AttributeSet attrs) {
        super(context, attrs);
        setup();
    }

    public MainView(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
        setup();
    }

    private void setup() {
        mConversationItems = new ArrayList<ConversationItem>();
    }

    public void setViewListener(ViewListener mViewListener) {
        this.mViewListener = mViewListener;
    }

    public void addConversationItem(ConversationItem c) {
        mConversationItems.add(c);
        mConversationAdapter.notifyDataSetChanged();
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();

        mConversationAdapter = new ConversationAdapter(getContext(),
                android.R.layout.simple_list_item_1, mConversationItems);

        mConversationView = (ListView) findViewById(R.id.conversationView);
        mConversationView.setAdapter(mConversationAdapter);
    }

    /**
     * This interface is used to send events from the view to the controller
     */
    public static interface ViewListener {

    }
}