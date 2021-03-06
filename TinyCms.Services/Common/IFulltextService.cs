namespace TinyCms.Services.Common
{
    /// <summary>
    ///     Full-Text service interface
    /// </summary>
    public interface IFulltextService
    {
        /// <summary>
        ///     Gets value indicating whether Full-Text is supported
        /// </summary>
        /// <returns>Result</returns>
        bool IsFullTextSupported();

        /// <summary>
        ///     Enable Full-Text support
        /// </summary>
        void EnableFullText();

        /// <summary>
        ///     Disable Full-Text support
        /// </summary>
        void DisableFullText();
    }
}