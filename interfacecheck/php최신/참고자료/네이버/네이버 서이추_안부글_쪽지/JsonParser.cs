using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 네이버_서이추_안부글_쪽지
{
    class NaverBlogCategory
    {
        public class MylogCategoryList
        {
            public bool categoryBlocked { get; set; }
            public string categoryType { get; set; }
            public int? parentCategoryNo { get; set; }
            public string categoryName { get; set; }
            public int categoryNo { get; set; }
            public bool openYN { get; set; }
            public int postCnt { get; set; }
            public int directorySeq { get; set; }
            public bool newPostExist { get; set; }
            public bool childCategory { get; set; }
            public bool divisionLine { get; set; }
        }

        public class MemologCategoryList
        {
            public bool categoryBlocked { get; set; }
            public string categoryType { get; set; }
            public object parentCategoryNo { get; set; }
            public string categoryName { get; set; }
            public int categoryNo { get; set; }
            public bool openYN { get; set; }
            public int postCnt { get; set; }
            public int directorySeq { get; set; }
            public bool newPostExist { get; set; }
            public bool childCategory { get; set; }
            public bool divisionLine { get; set; }
        }

        public class Result
        {
            public IList<MylogCategoryList> mylogCategoryList { get; set; }
            public IList<MemologCategoryList> memologCategoryList { get; set; }
            public int mylogPostCount { get; set; }
            public int memologPostCount { get; set; }
        }

        public class MainSer
        {
            public Result result { get; set; }
        }
    }

    class NaverBlogPosts
    {
        public class ThumbnailList
        {
            public string type { get; set; }
            public string encodedThumbnailUrl { get; set; }
            public bool vrthumbnail { get; set; }
        }

        public class PostViewList
        {
            public string blogId { get; set; }
            public object logNo { get; set; }
            public string titleWithInspectMessage { get; set; }
            public int commentCnt { get; set; }
            public int sympathyCnt { get; set; }
            public string briefContents { get; set; }
            public bool categoryOpenYn { get; set; }
            public bool categoryBlockYn { get; set; }
            public string categoryName { get; set; }
            public IList<ThumbnailList> thumbnailList { get; set; }
            public bool scraped { get; set; }
            public object addDate { get; set; }
            public int categoryNo { get; set; }
            public object openGraphLink { get; set; }
            public bool allOpenPost { get; set; }
            public bool memolog { get; set; }
            public bool outSideAllow { get; set; }
            public int scrapType { get; set; }
            public int smartEditorVersion { get; set; }
            public object readCount { get; set; }
            public object placeName { get; set; }
            public object thisDayPostInfo { get; set; }
            public bool bothBuddyOpen { get; set; }
            public bool sympathyArrowVisible { get; set; }
            public int thumbnailCount { get; set; }
            public bool commentArrowVisible { get; set; }
            public bool postBlocked { get; set; }
            public bool notOpen { get; set; }
            public bool buddyOpen { get; set; }
        }

        public class Result
        {
            public IList<PostViewList> postViewList { get; set; }
            public int currentPage { get; set; }
            public int totalCount { get; set; }
            public int categoryNo { get; set; }
            public string categoryName { get; set; }
            public int totalPage { get; set; }
        }

        public class MainSer
        {
            public Result result { get; set; }
        }
    }
}
